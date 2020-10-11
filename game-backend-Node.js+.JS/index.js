var express = require('express');
var app = express();
var bodyParser = require('body-parser');
var cors = require('cors');
var socketio = require("socket.io");
var http = require("http");
const { stringify } = require('querystring');
const SERVER_PORT = 8080;
app.set('port', (process.env.PORT || 5000));
app.use(bodyParser.json());
app.use(cors({
  // origin: true,
  // credentials: true,
  // allowedHeaders: true
}));

require('dotenv').config();
require('./db.js')(app);
require('./matches.js')(app);

let onlineClients = new Set();

const server = http.createServer(app);
const io = socketio(server);

const PORT = process.env.PORT || SERVER_PORT;
server.listen(PORT, () => console.info('Listening on port - ', PORT));

io.on('connection', onNewWebsocketConnection);


function onNewWebsocketConnection(socket) {
  console.info(`Socket ${socket.id} has connected.`);
  onlineClients.add(socket.id);
  //helper
  socket.on('game_play', function (data) {
    io.to(onlineClients[data.receiver].emit('game_play', data.msg))
  });

  //join to room
  socket.on("joinRoom", (data) => {
    socket.join(data);
    console.log(socket.id + ' join to room ' + data);
    console.log(socket.id + " now in rooms ", getRoomsByUser(socket.id));
    io.emit("connectRoom", data, socket.id);
  });
  //send to opponent for id
  socket.on("sendOpponent", (room, data) => {
    io.to(room).emit("receivedOpponent", data);
    console.log(socket.id + ' send opponent to ' + room);
  });

  //send ready
  socket.on("sendReady", (room) => {
    io.to(room).emit("sendReady");
    console.log(socket.id + ' ready in room ' + room);
  });
  //shotIn ship
  socket.on("shotIn", (data, opponent) => {
    io.to(`${opponent}`).emit("shotIn", data, opponent);
    console.log(socket.id + ' shotIn to ' + opponent);
  });
  // move turn to other player
  socket.on("setActive", (opponent) => {
    io.to(`${opponent}`).emit("setActive");
    console.log(socket.id + ' set active ' + opponent);
  });
  //game end
  socket.on("gameLose", (loser, game) => {
    io.to(game).emit("loseGame", loser);
    console.log(socket.id + ' lose game');
  });

  //send turn to special room
  socket.on("sendTurn", (data, opponent) => {
    console.log('turn ' + data + ' to ' + opponent);
    //io.emit("sendTurn", data);
    io.to(`${opponent}`).emit("sendTurn", data);
  }, function (answer) {
    console.log(answer);
  });
  //helper
  function getRoomsByUser(id) {
    let usersRooms = [];
    let rooms = io.sockets.adapter.rooms;

    for (let room in rooms) {
      if (rooms.hasOwnProperty(room)) {
        let sockets = rooms[room].sockets;
        if (id in sockets)
          usersRooms.push(room);
      }
    }

    return usersRooms;
  }

  socket.on("disconnect", () => {
    onlineClients.delete(socket.id);
    console.info(`Socket ${socket.id} has disconnected.`);
  });
};
