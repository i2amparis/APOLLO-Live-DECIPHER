"use strict";

function connectToGroup(hubUrl, groupName, onConnectedToGroup) {
    var connection = new signalR.HubConnectionBuilder()
        .withUrl(hubUrl)
        .withAutomaticReconnect()
        .build();

    connection.start()
        .then(function () {
            console.log(`Connected to 'hub:${hubUrl}'.`);

            connection.invoke("JoinGroup", groupName)
                .then(function () {
                    console.log(`Connected to 'group:${groupName}'.`);

                    if (onConnectedToGroup) {
                        onConnectedToGroup();
                    }
                })
                .catch(function (err) {
                    console.log.error(err.toString());
                });
            console.log(`Connected to 'hub:${hubUrl}:${groupName}'.`);
        }).catch(function (err) {
            return console.error(err.toString());
        });

    return connection;
}