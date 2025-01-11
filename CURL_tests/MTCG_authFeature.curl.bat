@echo off

REM Base URL and user credentials
set BASE_URL=http://127.0.0.1:10001
set USERNAME=flo
set PASSWORD=12345678
set INVALID_PASSWORD=wrongpassword
set AUTH_TOKEN=

REM Test Signup endpoint
echo Testing Signup Endpoint...
echo Case 1: Successful Signup
curl -s -o nul -w "%%{http_code}" -X POST %BASE_URL%/users ^
  -H "Content-Type: application/json" ^
  -d "{\"Username\": \"%USERNAME%\", \"Password\": \"%PASSWORD%\"}" > signup_result.txt
set /p SIGNUP_RESPONSE=<signup_result.txt
call :log_result "POST %BASE_URL%/users" "201 Created" "%SIGNUP_RESPONSE%"

echo Case 2: Signup with an existing user
curl -s -o nul -w "%%{http_code}" -X POST %BASE_URL%/users ^
  -H "Content-Type: application/json" ^
  -d "{\"Username\": \"%USERNAME%\", \"Password\": \"%PASSWORD%\"}" > signup_fail_result.txt
set /p SIGNUP_FAIL_RESPONSE=<signup_fail_result.txt
call :log_result "POST %BASE_URL%/users (existing user)" "4xx User already exists" "%SIGNUP_FAIL_RESPONSE%"

REM Test Login endpoint
echo Testing Login Endpoint...
echo Case 1: Successful Login
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

echo Case 2: Login with invalid password
curl -s -o nul -w "%%{http_code}" -X POST %BASE_URL%/sessions ^
  -H "Content-Type: application/json" ^
  -d "{\"Username\": \"%USERNAME%\", \"Password\": \"%INVALID_PASSWORD%\"}" > login_fail_result.txt
set /p LOGIN_FAIL_RESPONSE=<login_fail_result.txt
call :log_result "POST %BASE_URL%/sessions (invalid password)" "4xx Unauthorized" "%LOGIN_FAIL_RESPONSE%"

REM Test Logout endpoint
echo Testing Logout Endpoint...

echo Case 1: Logout without token
curl -s -o nul -w "%%{http_code}" -X DELETE %BASE_URL%/sessions > logout_fail_result.txt
set /p LOGOUT_FAIL_RESPONSE=<logout_fail_result.txt
call :log_result "DELETE %BASE_URL%/sessions (no token)" "4xx Unauthorized" "%LOGOUT_FAIL_RESPONSE%"

echo Case 2: Logout with invalid token
curl -s -o nul -w "%%{http_code}" -X DELETE %BASE_URL%/sessions ^
  -H "Authorization: invalid-token" > invalid_logout_result.txt
set /p INVALID_LOGOUT_RESPONSE=<invalid_logout_result.txt
call :log_result "DELETE %BASE_URL%/sessions (invalid token)" "4xx Unauthorized" "%INVALID_LOGOUT_RESPONSE%"

echo Case 3: Successful Logout
curl -s -o nul -w "%%{http_code}" -X DELETE %BASE_URL%/sessions ^
  -H "Authorization: %AUTH_TOKEN%" > logout_result.txt
set /p LOGOUT_RESPONSE=<logout_result.txt
call :log_result "DELETE %BASE_URL%/sessions" "200 OK" "%LOGOUT_RESPONSE%"


REM Cleanup temporary files
del *.txt 2>nul
del *.json 2>nul


REM Prevent CMD from closing immediately
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
