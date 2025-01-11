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
call :log_request "POST" "%BASE_URL%/users" "normaly fail but for safety it tries here again"
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
REM Test User Endpoints
REM -----------------------------------------------------------------
echo Testing User Endpoints...

REM GetUser
echo Case 1: GetUser - Success
call :log_request "GET" "%BASE_URL%/users/%USERNAME%" "200 OK"
curl -s -X GET %BASE_URL%/users/%USERNAME% ^
  -H "Authorization: %AUTH_TOKEN%" > user_response.txt
type user_response.txt
echo.

echo Case 2: GetUser - Unauthorized
call :log_request "GET" "%BASE_URL%/users/%USERNAME%" "401 Unauthorized"
curl -s -X GET %BASE_URL%/users/%USERNAME% ^
  -H "Authorization: %INVALID_AUTH_TOKEN%" > user_fail_response.txt
type user_fail_response.txt
echo.

echo.
REM GetStats
echo Case 1: GetStats - Success
call :log_request "GET" "%BASE_URL%/stats" "200 OK"
curl -s -X GET %BASE_URL%/stats ^
  -H "Authorization: %AUTH_TOKEN%" > stats_response.txt
type stats_response.txt
echo.

echo.
REM GetStats
echo Case 2: GetStats - Unauthorized
call :log_request "GET" "%BASE_URL%/stats" "401 Unauthorized"
curl -s -X GET %BASE_URL%/stats ^
  -H "Authorization: %INVALID_AUTH_TOKEN%" > stats_response.txt
type stats_response.txt
echo.

echo.
REM -----------------------------------------------------------------
REM Test Transactions Endpoints
REM -----------------------------------------------------------------
echo Testing Transactions Endpoints...

echo.
REM buyPackage
echo Case 1: BuyPackage - Success
call :log_request "POST" "%BASE_URL%/transactions/packages" "201 Created"
curl -s -X POST %BASE_URL%/transactions/packages ^
  -H "Authorization: %AUTH_TOKEN%" > buy_package_response.txt
type buy_package_response.txt
echo.

echo.
REM buyPackage
echo Case 2: BuyPackage - Unauthorized
call :log_request "POST" "%BASE_URL%/transactions/packages" "401 Unauthorized"
curl -s -X POST %BASE_URL%/transactions/packages ^
  -H "Authorization: %INVALID_AUTH_TOKEN%" > buy_package_response.txt
type buy_package_response.txt
echo.

echo.
REM -----------------------------------------------------------------
REM Test Cards Endpoints
REM -----------------------------------------------------------------
echo Testing Cards Endpoints...

echo.
REM GetStackFromUser
echo Case 2: GetStackFromUser - Unauthorized
call :log_request "GET" "%BASE_URL%/cards" "401 Unauthorized"
curl -s -X GET %BASE_URL%/cards ^
  -H "Authorization: %INVALID_AUTH_TOKEN%" > cards_response.txt
type cards_response.txt
echo.

echo.
REM GetStackFromUser
echo Case 1: GetStackFromUser - Success
call :log_request "GET" "%BASE_URL%/cards" "200 OK"
curl -s -X GET %BASE_URL%/cards ^
  -H "Authorization: %AUTH_TOKEN%" > cards_response.txt
type cards_response.txt
echo.

setlocal enabledelayedexpansion

REM File containing the JSON response
set FILE=cards_response.txt

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
REM GetDeck
echo GetDeck - Empty
call :log_request "GET" "%BASE_URL%/deck" "204 No Content"
curl -s -X GET %BASE_URL%/deck ^
  -H "Authorization: %AUTH_TOKEN%" > get_deck_response.txt
type get_deck_response.txt
echo.

echo.
REM PutDeck
echo Case 1: PutDeck - Success
call :log_request "PUT" "%BASE_URL%/deck" "200 OK"
curl -s -X PUT %BASE_URL%/deck ^
  -H "Authorization: %AUTH_TOKEN%" ^
  -H "Content-Type: application/json" ^
  -d "{ \"cards\": [%ID%, %ID1%, %ID2%, %ID3%] }" > put_deck_response.txt
type put_deck_response.txt
echo.

echo.
REM PutDeck
echo Case 2: PutDeck - Failure
call :log_request "PUT" "%BASE_URL%/deck" "403 Forbidden"
curl -s -X PUT %BASE_URL%/deck ^
  -H "Authorization: %AUTH_TOKEN%" ^
  -H "Content-Type: application/json" ^
  -d "{ \"cards\": [%ID%, %ID1%, %ID2%, 10000] }" > put_deck_response.txt
type put_deck_response.txt
echo.

echo.
REM PutDeck
echo Case 3: PutDeck - Unauthorized
call :log_request "PUT" "%BASE_URL%/deck" "401 Unauthorized"
curl -s -X PUT %BASE_URL%/deck ^
  -H "Authorization: %INVALID_AUTH_TOKEN%" ^
  -H "Content-Type: application/json" ^
  -d "{ \"cards\": [%ID%, %ID1%, %ID2%, %ID3%] }" > put_deck_response.txt
type put_deck_response.txt
echo.

echo.
REM GetDeck
echo Case 1: GetDeck - Success
call :log_request "GET" "%BASE_URL%/deck" "200 OK"
curl -s -X GET %BASE_URL%/deck ^
  -H "Authorization: %AUTH_TOKEN%" > get_deck_response.txt
type get_deck_response.txt
echo.

echo.
REM GetDeck
echo Case 2: GetDeck - Unauthorized
call :log_request "GET" "%BASE_URL%/deck" "401 Unauthorized"
curl -s -X GET %BASE_URL%/deck ^
  -H "Authorization: %INVALID_AUTH_TOKEN%" > get_deck_response.txt
type get_deck_response.txt
echo.

echo.
REM -----------------------------------------------------------------
REM Test Shop Endpoints
REM -----------------------------------------------------------------
echo Testing Shop Endpoints...

echo.
REM GetShopItems
echo Case 1: GetShopItems - Success
call :log_request "GET" "%BASE_URL%/shopitem" "200 OK"
curl -s -X GET %BASE_URL%/shopitem ^
  -H "Authorization: %AUTH_TOKEN%" > shop_items_response.txt
type shop_items_response.txt
echo.

echo.
REM GetShopItems
echo Case 2: GetShopItems - Unauthorized
call :log_request "GET" "%BASE_URL%/shopitem" "401 Unauthorized"
curl -s -X GET %BASE_URL%/shopitem ^
  -H "Authorization: %INVALID_AUTH_TOKEN%" > shop_items_response.txt
type shop_items_response.txt
echo.

echo.
REM BuyShopItem
echo Case 1: BuyShopItem - Success
call :log_request "POST" "%BASE_URL%/shopitem/1" "200 OK"
curl -s -X POST %BASE_URL%/shopitem/1 ^
  -H "Authorization: %AUTH_TOKEN%" > buy_shop_item_response.txt
type buy_shop_item_response.txt
echo.

echo.
REM BuyShopItem
echo Case 2: BuyShopItem - Unauthorized
call :log_request "POST" "%BASE_URL%/shopitem/1" "401 Unauthorized"
curl -s -X POST %BASE_URL%/shopitem/1 ^
  -H "Authorization: %INVALID_AUTH_TOKEN%" > buy_shop_item_response.txt
type buy_shop_item_response.txt
echo.

echo.
REM BuyShopItem
echo Case 3: BuyShopItem - Not Found
call :log_request "POST" "%BASE_URL%/shopitem/100" "403 Not Found"
curl -s -X POST %BASE_URL%/shopitem/100 ^
  -H "Authorization: %AUTH_TOKEN%" > buy_shop_item_response.txt
type buy_shop_item_response.txt
echo.

echo.
REM -----------------------------------------------------------------
REM Test Not enough coins results
REM -----------------------------------------------------------------
echo Test Not enough coins results

echo.
REM buyPackage
echo Should work, 5 coins are left
call :log_request "POST" "%BASE_URL%/transactions/packages" "200 OK"
curl -s -X POST %BASE_URL%/transactions/packages ^
  -H "Authorization: %AUTH_TOKEN%" > buy_package_response.txt
type buy_package_response.txt
echo.

echo.
REM buyPackage
echo could work, depends on mystery package
call :log_request "POST" "%BASE_URL%/transactions/packages" ""
curl -s -X POST %BASE_URL%/transactions/packages ^
  -H "Authorization: %AUTH_TOKEN%" > buy_package_response.txt
type buy_package_response.txt
echo.

echo.
REM buyPackage
echo could work, depends on mystery package
call :log_request "POST" "%BASE_URL%/transactions/packages" ""
curl -s -X POST %BASE_URL%/transactions/packages ^
  -H "Authorization: %AUTH_TOKEN%" > buy_package_response.txt
type buy_package_response.txt
echo.

echo.
REM BuyShopItem
echo Case 1: BuyShopItem - Not enough coins
call :log_request "POST" "%BASE_URL%/shopitem/1" "403 Forbidden"
curl -s -X POST %BASE_URL%/shopitem/1 ^
  -H "Authorization: %AUTH_TOKEN%" > buy_shop_item_response.txt
type buy_shop_item_response.txt
echo.

echo.
REM buyPackage
echo Case 2: BuyPackage - Not enough coins
call :log_request "POST" "%BASE_URL%/transactions/packages" "403 Forbidden"
curl -s -X POST %BASE_URL%/transactions/packages ^
  -H "Authorization: %AUTH_TOKEN%" > buy_package_response.txt
type buy_package_response.txt
echo.

echo.
REM -----------------------------------------------------------------
REM See updates on User (coins and cards)
REM -----------------------------------------------------------------
echo Test Not enough coins results
REM GetUser
echo Case 1: GetUser - Success
call :log_request "GET" "%BASE_URL%/users/%USERNAME%" "OK"
curl -s -X GET %BASE_URL%/users/%USERNAME% ^
  -H "Authorization: %AUTH_TOKEN%" > user_response.txt
type user_response.txt
echo.

REM Cleanup temporary files
del *.txt 2>nul
del *.json 2>nul

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