function read(query, user, request) {

    console.log("Content: " + request.parameters.version);
    if (request.parameters.version == "2.0") {
        console.log("Do V2.0 specific thing");
    }
    request.execute();
}

function insert(item, user, request) {
    //request.execute();

    request.execute({
        success: function () {
            // Write to the response and then send the notification in the background
            request.respond();
            push.wns.sendToastText04(item.channel, {
                text1: item.text
            }, {
                success: function (pushResponse) {
                    console.log("Sent push:", pushResponse);
                }
            });
        }
    });
}

