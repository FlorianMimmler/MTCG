@echo off

REM Base URL and user credentials
set BASE_URL=http://127.0.0.1:10001
set USERNAME=flo
set PASSWORD=12345678
set AUTH_TOKEN=
set INVALID_AUTH_TOKEN=invalid-auth-token

REM ---------------
REM SETUP (Create User and Login)
REM ---------------
echo Signup User...
call :log_request "POST" "%BASE_URL%/users" "201 Created"
curl -s -o nul -w "%%{http_code}" -X POST %BASE_URL%/users ^
  -H "Content-Type: application/json" ^
  -d "{\"Username\": \"%USERNAME%\", \"Password\": \"%PASSWORD%\"}" > signup_result.txt
set /p SIGNUP_RESPONSE=<signup_result.txt
call :log_result "POST %BASE_URL%/users" "201 Created" "%SIGNUP_RESPONSE%"

echo Login User and extract AuthToken
call :log_request "POST" "%BASE_URL%/sessions" "200 OK"
curl -s -X POST %BASE_URL%/sessions ^
  -H "Content-Type: application/json" ^
  -d "{\"Username\": \"%USERNAME%\", \"Password\": \"%PASSWORD%\"}" > login_response.json

REM Extract the token from the response
for /f "tokens=2 delims=:}" %%A in ('findstr "Token" login_response.json') do set AUTH_TOKEN=%%~A
set AUTH_TOKEN=%AUTH_TOKEN%

REM Check if the token was extracted
if "%AUTH_TOKEN%"=="" (
  echo Error: Token could not be extracted. Response might be invalid.
  goto :eof
)

echo Received Token: %AUTH_TOKEN%


echo.
REM -----------------------------------------------------------------
REM Test AI Battle
REM -----------------------------------------------------------------
echo Testing AI Battle

REM AI Battle
echo Case 1: AI Battle - Success
call :log_request "POST" "%BASE_URL%/ai-battle" "200 OK"
curl -s -X POST %BASE_URL%/ai-battle ^
  -H "Authorization: %AUTH_TOKEN%" > ai_battle_response.txt
type ai_battle_response.txt
echo.

echo.
echo Case 2: AI Battle - Unauthorized
call :log_request "POST" "%BASE_URL%/ai-battle" "401 Unauthorized"
curl -s -X POST %BASE_URL%/ai-battle ^
  -H "Authorization: %INVALID_AUTH_TOKEN%" > ai_battle_response.txt
type ai_battle_response.txt
echo.

echo.
REM -----------------------------------------------------------------
REM Test Battle, when script stops here and waits, start the MTCG_battle2.curl.bat
REM -----------------------------------------------------------------
echo Testing Battle

REM Battle
echo Case 1: Battle - Success
call :log_request "POST" "%BASE_URL%/battles" "200 OK"
curl -s -X POST %BASE_URL%/battles ^
  -H "Authorization: %AUTH_TOKEN%" > battle1_1_response.json
type battle1_1_response.json
echo.

echo.
echo Case 2: Battle - Unauthorized
call :log_request "POST" "%BASE_URL%/battles" "401 Unauthorized"
curl -s -X POST %BASE_URL%/battles ^
  -H "Authorization: %INVALID_AUTH_TOKEN%" > battle1_2_response.txt
type battle1_2_response.txt
echo.


REM Cleanup temporary files
del *.txt 2>nul

echo Tests completed. Press any key to exit...
pause
goto :eof

REM Helper function to log results
:log_result
echo ----------------------------------
echo Request: %1
echo Expected: %2
echo Actual: %3
echo ----------------------------------
echo.
exit /b

REM Helper function to log requests
:log_request
echo ----------------------------------
echo Request Type: %1
echo URL: %2
echo Expected: %3
echo.
exit /b