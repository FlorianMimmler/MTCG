@echo off

REM Base URL and user credentials
set BASE_URL=http://127.0.0.1:10001
set USERNAME=flo
set USERNAME2=otherPlayer
set PASSWORD=12345678
set AUTH_TOKEN=
set AUTH_TOKEN2=
set INVALID_AUTH_TOKEN=invalid-auth-token

set CardToTrade=
set CardToRequestTrade=
set Card2ToRequestTrade=

REM ---------------
REM SETUP (Create Users and Login)
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

echo Signup User...
call :log_request "POST" "%BASE_URL%/users" "201 Created"
curl -s -o nul -w "%%{http_code}" -X POST %BASE_URL%/users ^
  -H "Content-Type: application/json" ^
  -d "{\"Username\": \"%USERNAME2%\", \"Password\": \"%PASSWORD%\"}" > signup2_result.txt
set /p SIGNUP_RESPONSE=<signup2_result.txt
call :log_result "POST %BASE_URL%/users" "201 Created" "%SIGNUP2_RESPONSE%"

echo Login User and extract AuthToken
call :log_request "POST" "%BASE_URL%/sessions" "200 OK"
curl -s -X POST %BASE_URL%/sessions ^
  -H "Content-Type: application/json" ^
  -d "{\"Username\": \"%USERNAME2%\", \"Password\": \"%PASSWORD%\"}" > login2_response.json

REM Extract the token from the response
for /f "tokens=2 delims=:}" %%A in ('findstr "Token" login2_response.json') do set AUTH_TOKEN2=%%~A
set AUTH_TOKEN2=%AUTH_TOKEN2%

REM Check if the token was extracted
if "%AUTH_TOKEN2%"=="" (
  echo Error: Token could not be extracted. Response might be invalid.
  goto :eof
)

echo Received Token: %AUTH_TOKEN2%

echo.
REM -----------------------------------------------------------------
REM Setup Get Card IDs for trading
REM -----------------------------------------------------------------
echo Setup Get Card IDs for trading

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
set CardToTrade=%ID: =%

REM Print the extracted ID
echo Extracted ID: %CardToTrade%

echo.
REM GetStackFromUser
echo Case 1: GetStackFromUser - Success
call :log_request "GET" "%BASE_URL%/cards" "200 OK"
curl -s -X GET %BASE_URL%/cards ^
  -H "Authorization: %AUTH_TOKEN2%" > cards2_response.txt
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
set CardToRequestTrade=%ID: =%
set /a Card2ToRequestTrade=%CardToRequestTrade%-1

REM Print the extracted ID
echo Extracted ID: %CardToRequestTrade%
echo Extracted ID: %Card2ToRequestTrade%


echo.
REM -----------------------------------------------------------------
REM Test Trading
REM -----------------------------------------------------------------
echo Testing Trading


REM 1. CreateTrade - Success
echo 1. CreateTrade - Success
call :log_request "POST" "%BASE_URL%/trade" "201 Created"
curl -s -X POST %BASE_URL%/trade ^
  -H "Authorization: %AUTH_TOKEN%" ^
  -H "Content-Type: application/json" ^
  -d "{ \"OfferedCardId\": %CardToTrade%, \"Requirements\": { \"CardType\": 0, \"ElementType\": 0, \"MonsterType\": 1, \"MinDamage\": 70 } }" > create_trade_response.txt
type create_trade_response.txt
echo.

REM 2. CreateTrade - Unauthorized
echo 2. CreateTrade - Unauthorized
call :log_request "POST" "%BASE_URL%/trade" "401 Unauthorized"
curl -s -X POST %BASE_URL%/trade ^
  -H "Authorization: %INVALID_AUTH_TOKEN%" ^
  -H "Content-Type: application/json" ^
  -d "{ \"OfferedCardId\": %CardToTrade%, \"Requirements\": { \"CardType\": 0, \"ElementType\": 0, \"MonsterType\": 1, \"MinDamage\": 70 } }" > create_trade_unauthorized_response.txt
type create_trade_unauthorized_response.txt
echo.

REM 3. GetTrades - Success
echo 3. GetTrades - Success
call :log_request "GET" "%BASE_URL%/trade" "200 OK"
curl -s -X GET %BASE_URL%/trade ^
  -H "Authorization: %AUTH_TOKEN%" > get_trades_response.txt
type get_trades_response.txt
echo.

REM 4. CreateTradeRequest - Success
echo 4. CreateTradeRequest - Success
call :log_request "POST" "%BASE_URL%/tradeRequest" "201 Created"
curl -s -X POST %BASE_URL%/tradeRequest ^
  -H "Authorization: %AUTH_TOKEN2%" ^
  -H "Content-Type: application/json" ^
  -d "{\"OfferedCardId\": %CardToRequestTrade%,\"TradeId\": 0 }" > create_trade_request_response.txt
type create_trade_request_response.txt
echo.

REM 4.1 CreateTradeRequest2 - Success
echo 4.1 CreateTradeRequest2 - Success
call :log_request "POST" "%BASE_URL%/tradeRequest" "201 Created"
curl -s -X POST %BASE_URL%/tradeRequest ^
  -H "Authorization: %AUTH_TOKEN2%" ^
  -H "Content-Type: application/json" ^
  -d "{\"OfferedCardId\": %Card2ToRequestTrade%,\"TradeId\": 0 }" > create_trade_request2_response.txt
type create_trade_request2_response.txt
echo.

REM 4.2 CreateTradeRequest2 - Failure
echo 4.2 CreateTradeRequest2 - Failure
call :log_request "POST" "%BASE_URL%/tradeRequest" "403 Forbidden"
curl -s -X POST %BASE_URL%/tradeRequest ^
  -H "Authorization: %AUTH_TOKEN%" ^
  -H "Content-Type: application/json" ^
  -d "{\"OfferedCardId\": %CardToTrade%, \"TradeId\": 0 }" > create_trade_request3_response.txt
type create_trade_request3_response.txt
echo.

REM 5. GetTradesByUser - Success
echo 5. GetTradesByUser - Success
call :log_request "GET" "%BASE_URL%/trade/%USERNAME%" "200 OK"
curl -s -X GET %BASE_URL%/trade/%USERNAME% ^
  -H "Authorization: %AUTH_TOKEN%" > get_trades_by_user_response.txt
type get_trades_by_user_response.txt
echo.

REM 6. GetTradeRequestsForTrade - Success
echo 6. GetTradeRequestsForTrade - Success
call :log_request "GET" "%BASE_URL%/trade/0/tradeRequests" "200 OK"
curl -s -X GET %BASE_URL%/trade/0/tradeRequests ^
  -H "Authorization: %AUTH_TOKEN%" > get_trade_requests_response.txt
type get_trade_requests_response.txt
echo.

REM 7. AcceptTrade - Success
echo 7. AcceptTrade - Success
call :log_request "PUT" "%BASE_URL%/tradeRequest/1" "200 OK"
curl -s -X PUT %BASE_URL%/tradeRequest/1 ^
  -H "Authorization: %AUTH_TOKEN%" > accept_trade_response.txt
type accept_trade_response.txt
echo.

REM 8. AcceptTrade - Unauthorized
echo 8. AcceptTrade - Unauthorized
call :log_request "PUT" "%BASE_URL%/tradeRequest/2" "401 Unauthorized"
curl -s -X PUT %BASE_URL%/tradeRequest/2 ^
  -H "Authorization: %INVALID_AUTH_TOKEN%" > accept_trade_unauthorized_response.txt
type accept_trade_unauthorized_response.txt
echo.

echo.
echo Test if trade was successful, card ids %CardToTrade% and %Card2ToRequestTrade% should have changed
REM GetStackFromUser
echo Cards from %USERNAME%
call :log_request "GET" "%BASE_URL%/cards" "200 OK"
curl -s -X GET %BASE_URL%/cards ^
  -H "Authorization: %AUTH_TOKEN%" > cards_response.txt
type cards_response.txt
echo.

echo.
REM GetStackFromUser
echo Cards from %USERNAME2%
call :log_request "GET" "%BASE_URL%/cards" "200 OK"
curl -s -X GET %BASE_URL%/cards ^
  -H "Authorization: %AUTH_TOKEN2%" > cards2_response.txt
type cards2_response.txt
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