

#r "../packages/FSharp.Data.2.1.0/lib/net40/FSharp.Data.dll"
open FSharp.Data
open FSharp.Data.HttpRequestHeaders
open System

let scoreTopic = "player.score_update"
let gameEndTopic = "game.end"

type Subscription = JsonProvider<"""{"retrieval_url": "http://combo-squirrel.herokuapp.com/topics/chat/subscriptions/amq.gen-CRx_i-xTQvqsqthuUDNYAw/next", "subscription_id": "amq.gen-CRx_i-xTQvqsqthuUDNYAw"}""">
type GameEndMessage = JsonProvider<"""{ "game": 32, "result": "win", "winner": "player_uuid"}"""> 

let SubscribeToTopic topic =
    let subsripction = Http.RequestString( "http://combo-squirrel.herokuapp.com/topics/" + topic + "/subscriptions", 
        headers = [ ContentType HttpContentTypes.Json ],
        body = TextRequest "")
    let sub = Subscription.Parse subsripction
    sub.SubscriptionId

let gameSubscriptionId = SubscribeToTopic gameEndTopic
let gameSubscriptionUrl = String.Format( "http://combo-squirrel.herokuapp.com/topics/{0}/subscriptions/{1}/next", gameEndTopic, gameSubscriptionId)

let latestMessage = Http.RequestString(gameSubscriptionUrl)

let processGameEnd message =
    let gameEnd = GameEndMessage.Parse(latestMessage)
    if gameEnd.Result = "win" then
        //update player score and send the message

if latestMessage <> "" then        
        processGameEnd latestMessage
    




let replyToLatestMessage (who :string) = 
    if who <> "Adam's Bot" then
        let botResponse = "Hi " + who
        let chatResponseBot = """ {"who": "Adam's Bot", "says":""" + "\"" + botResponse + "\"}"
        Http.RequestString
          ( "http://combo-squirrel.herokuapp.com/topics/chat/facts", 
            headers = [ ContentType HttpContentTypes.Json ],
            body = TextRequest chatResponseBot )
    else
        ""

let processLatestSubscription latestMsg = 
    if latestMsg <> "" then
        let msg = ChatMessage.Parse(latestMsg)
        replyToLatestMessage msg.Who
    else
        ""

let subscriptionChats = Http.RequestString(chatSubscriptionUrl)
processLatestSubscription subscriptionChats


while Http.RequestString(chatSubscriptionUrl) <> "" do
    ignore

    //
//Http.RequestString
//  ( "http://combo-squirrel.herokuapp.com/topics/chat/facts", 
//    headers = [ ContentType HttpContentTypes.Json ],
//    body = TextRequest """ {"who": "adam", "says":"hi"} """)


//Http.RequestString("http://combo-squirrel.herokuapp.com/topics/chat/facts")