@echo off

REM Base URL and user credentials
set BASE_URL=http://127.0.0.1:10001
set USERNAME=otherPlayer
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
  -d "{\"Username\": \"%USERNAME%\", \"Password\": \"%PASSWORD%\"}" > signup2_result.txt
set /p SIGNUP_RESPONSE=<signup2_result.txt
call :log_result "POST %BASE_URL%/users" "201 Created" "%SIGNUP_RESPONSE%"

echo Login User and extract AuthToken
call :log_request "POST" "%BASE_URL%/sessions" "200 OK"
curl -s -X POST %BASE_URL%/sessions ^
  -H "Content-Type: application/json" ^
  -d "{\"Username\": \"%USERNAME%\", \"Password\": \"%PASSWORD%\"}" > login2_response.json

REM Extract the token from the response
for /f "tokens=2 delims=:}" %%A in ('findstr "Token" login2_response.json') do set AUTH_TOKEN=%%~A
set AUTH_TOKEN=%AUTH_TOKEN%

REM Check if the token was extracted
if "%AUTH_TOKEN%"=="" (
  echo Error: Token could not be extracted. Response might be invalid.
  goto :eof
)

echo Received Token: %AUTH_TOKEN%

echo.
REM buyPackage
echo Case 2: BuyPackage - success
call :log_request "POST" "%BASE_URL%/transactions/packages" "200 OK"
curl -s -X POST %BASE_URL%/transactions/packages ^
  -H "Authorization: %AUTH_TOKEN%" > buy_package2_response.txt
type buy_package2_response.txt
echo.

echo.
REM GetStackFromUser
echo Case 1: GetStackFromUser - Success
call :log_request "GET" "%BASE_URL%/cards" "200 OK"
curl -s -X GET %BASE_URL%/cards ^
  -H "Authorization: %AUTH_TOKEN%" > cards2_response.txt
type cards2_response.txt
echo.

setlocal enabledelayedexpansion

REM File containing the JSON response
set FILE=cards2_response.txt

REM Read the first line of the file
set LINE=
for /f "delims=" %%A in (%FILE%) do (
    set LINE=%%A
    goto :read_done
)
:read_done

REM Find the first ':' and extract everything after it
for /f "tokens=2 delims=:" %%A in ("!LINE!") do set PART=%%A

REM Extract the portion before the first ','
for /f "tokens=1 delims=," %%A in ("!PART!") do set ID=%%A

REM Remove any surrounding spaces (if applicable)
set ID=%ID: =%
set /a ID1=%ID%+1
set /a ID2=%ID%+2
set /a ID3=%ID%+3

REM Print the extracted ID
echo Extracted ID: %ID%


echo.
REM -----------------------------------------------------------------
REM Test Battle without Deck
REM -----------------------------------------------------------------
echo Testing Battle without Deck

REM Battle
echo Case 1: Battle - Failure (No Deck)
call :log_request "POST" "%BASE_URL%/battles" "404"
curl -s -X POST %BASE_URL%/battles ^
  -H "Authorization: %AUTH_TOKEN%" > battle2_1_response.txt
type battle2_1_response.txt
echo.

echo.
REM PutDeck
echo PutDeck - Success
call :log_request "PUT" "%BASE_URL%/deck" "200 OK"
curl -s -X PUT %BASE_URL%/deck ^
  -H "Authorization: %AUTH_TOKEN%" ^
  -H "Content-Type: application/json" ^
  -d "{ \"cards\": [%ID%, %ID1%, %ID2%, %ID3%] }" > put_deck_response.txt
type put_deck_response.txt
echo.

REM Battle
echo Case 1: Battle - Success
call :log_request "POST" "%BASE_URL%/battles" "200 OK"
curl -s -X POST %BASE_URL%/battles ^
  -H "Authorization: %AUTH_TOKEN%" > battle2_2_response.json
type battle2_2_response.json
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