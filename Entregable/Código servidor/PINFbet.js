
// Subobjectives: SO_4, SO_6, SO_7 SO_9.

//==========[ Basic NodeJs and Express Components ]============
const { json } = require('body-parser');
var express = require('express');
var app = express();

//====================[ External Modules ]=====================
var mysql = require('mysql');
var unless = require('express-unless');
var config = require('./config');
var auth = require('./authentication');
var middleware = require('./middleware');
const { EMLINK } = require('constants');

//====================[ System Setup ]=========================
app.use(express.json())

//The database should use a user which only allows connections from the local machine
//to prevent security issues. Already done in the default PinfBetDB parameters.
var con = mysql.createConnection({
  //To change the connection options modify the config.js file
  host: config.DBhost,
  user: config.DBuser,
  password: config.DBpassword,
  database: config.database
});

//Connect and inform about the database
con.connect(function(err) {
  if (err) throw err;
  console.log("DataBase Connected!");
});

//====================[ Middleware Initialization ]=========================

//We will only accept request which have a valid authentication token header
//this doesn't apply to public methods such as login and creation of users.
//It also adds the req.user and req.userId fields containing the name and id of the user
middleware.ensureAuthenticated.unless = unless;
app.use(middleware.ensureAuthenticated.unless({
  path: [
    /\/checkUser\/(.+)/,
    /\/createAccount/,
    /\/loginAccount\/(.+)/
  ]})
);
//We will also provide information on the console about the request received
app.use(middleware.info)

//====================[ General Public Module ]=========================
//Returns the salt used to encrypt the provided username, which is useless without
//knowing the secret password since the password is asymmetrically encrypted
app.get('/checkUser/:name', function (req, res) {
  var name = req.params.name;
  var sql = `SELECT salt FROM Users WHERE username = "${name}"`;
  con.query(sql, function(err, result){
    if(err) {
      res.status(404).end();
    }else{
      if (result.length == 0) res.status(404).end();
      else res.status(200).send(result[0].salt);
    }
  })
})

//Creates a new account with the data provided
app.post('/createAccount',function (req, res){
  var user = req.body;
  var sql = `INSERT INTO Users VALUES ("${user.name}", "${user.password}", "${user.salt}", "${user.fullname}", NULL, 0, 0)`;
  con.query(sql, function(err, result){
    if(err) {
      res.status(400).end();
    }else{
      if (result.length == 0) res.status(404).end();
      else res.status(200).send(auth.createToken(user.name, result.insertId));
    }
  })
})

//Checks wheter the account exists to log in
app.get('/loginAccount/:name', function (req, res) {
  var pass = req.headers.authorization;
  var sql = `SELECT * FROM Users WHERE username = "${req.params.name}" AND password = "${pass}"`;
  con.query(sql, function(err, result){
    if(err) {
      res.status(404).end();
    }else{
      if (result.length == 0) res.status(404).end();
      else res.status(200).send(auth.createToken(result[0].username, result[0].id));
    }
  })
})

//====================[ General Module Protected ]=========================
//All the following request are automatically provided with req.user containing the username
//and req.userId containing the id of the user because of the ensureAuthentication middleware

//Responds with the isUpdated of the user, which indicates if the user should be asked to update the data stored
app.get('/isUpdated', function (req, res) {
  var sql = `SELECT isUpdated FROM Users WHERE id = "${req.userId}"`;
  con.query(sql, function(err, result){
    if(err) {
      res.status(404).end();
    }else{
      res.status(200).send(JSON.stringify(result[0].isUpdated));
    }
  })
})

//Responds with the coins of the user provided.
app.get('/getCoins', function (req, res) {
  var sql = `SELECT coins FROM Users WHERE id = "${req.userId}"`;
  con.query(sql, function(err, result){
    if(err) {
      res.status(404).end();
    }else{
      res.status(200).send(JSON.stringify(result[0].coins));
    }
  })
})

//Responds with all subjects which the user has not passed already.
app.get('/getAvailableSubjects',function (req, res){
  var sql = `SELECT sub.name, sub.course FROM Subjects sub LEFT JOIN Studying std ON sub.name = std.subjectname AND std.studentname = "${req.user}" LEFT JOIN Marks mar ON std.studentname = mar.studentname AND std.subjectname = mar.subjectname WHERE mar.markObtained IS NULL OR mar.markObtained < 5`;
  con.query(sql, function(err, result, fields){
    if(err) {
      res.status(404).end();
    }else{
      var toSend = {"primero":[],"segundo":[],"tercero":[],"cuarto":[]};
      result.forEach(element => {
          toSend[element.course].push(element.name);
      });
      console.log(JSON.stringify(toSend));
      res.status(200).send(JSON.stringify(toSend));
    }
  })
})

//Responds with the subjects which the user has finished or is currently studying
app.get('/getSubjects',function (req, res){
  var sql = `SELECT subjectname FROM Studying WHERE studentname = "${req.user}"`;
  con.query(sql, function(err, result, fields){
    if(err) {
      res.status(404).end();
    }else{
      var toSend = [];
      result.forEach(element => {
          toSend.push(element.subjectname);
      });
      res.status(200).send(JSON.stringify(toSend));
    }
  })
})

//Responds with the subjects which the user is studying and has not passed yet
app.get('/getSubjectsToMark',function (req, res){
  var sql = `SELECT s.subjectname FROM Studying s LEFT JOIN Marks m ON s.studentname = m.studentname AND s.subjectname = m.subjectname WHERE s.studentname = "${req.user}" AND m.markObtained IS NULL;`;
  con.query(sql, function(err, result, fields){
    if(err) {
      res.status(404).end();
    }else{
      var toSend = [];
      result.forEach(element => {
          toSend.push(element.subjectname);
      });
      console.log(JSON.stringify(toSend));
      res.status(200).send(JSON.stringify(toSend));
    }
  })
})

//Adds the provided subjects to the subjects the user is currently studying
app.post('/addSubjects',function (req, res){
  var sql = `INSERT INTO Studying (subjectname, studentname) VALUES ?`;
  var values = [];
  req.body.names.forEach(element =>{
    values.push([element, req.user]);
  })
  con.query(sql, [values], function(err, result, fields){
    if(err) {
      res.status(404).end();
    }else{
      res.status(200).end();
    }
  })
})

//Adds the provided marks to the user, then updates the user's coins based on credits obtained and average mark.
//Updates all the coins of the users which betted on the provided subjects of this username based on how close the bet was.
//Deletes all the bets made on those subjects and sets the user as updated.
app.post('/addMarks',function (req, res){
  var values = [];
  var subjects = "";
  req.body.forEach(element =>{
    values.push([req.user, element.Key, element.Value]);
    subjects += `"${element.Key}",`;
  })
  if(values.length == 0) {res.status(200).end()}
  else{
    subjects = subjects.slice(0,-1);
    //Insert new marks
    var sql = `INSERT INTO Marks (studentname, subjectname, markObtained) VALUES ? AS m ON DUPLICATE KEY UPDATE markObtained = m.markObtained`;
    con.query(sql, [values], function(err, result, fields){
      if(err) {
        console.log(err);
        res.status(404).end();
      }else{
        //Update coins for user based on obtained credits and average mark
        con.query(`UPDATE Users SET coins = coins + ROUND(6*(SELECT AVG(markObtained) FROM Marks WHERE studentname = "${req.user}")*(SELECT Count(*) FROM Marks WHERE studentname = "${req.user}" AND markObtained >= 5 AND subjectname IN (${subjects}))) WHERE id = ${req.userId};`)
        //Update coins for everyone based on bets made
        var sql = `UPDATE Users u LEFT JOIN Bets b ON u.username = b.username LEFT JOIN Marks m ON (b.studentname = m.studentname AND b.subjectname = m.subjectname) SET u.coins = u.coins + ROUND(b.coins*GREATEST(0, 2-ABS(b.markExpected-m.markObtained))) WHERE (b.username IS NOT NULL OR b.username <> "${req.user}") AND m.studentname is NOT NULL;`
        con.query(sql, [values], function(err, result, fields){
          if(err){
            res.status(400).end(error);
          }else{
            console.log("\n DELETE REACHED");
            var delsql = `DELETE FROM Bets WHERE studentname = "${req.user}" AND subjectname IN (${subjects});`;
            console.log(delsql)
            con.query(delsql, function(err, result, fields){
              if(err){
                res.status(400).end(error);
              }else{
                con.query(`UPDATE Users SET isUpdated = 1 WHERE id = ${req.userId}`);
                res.status(200).end();
              }
            });
          }
        });
      }
    });
  }
});

//====================[ Friends Module ]=========================
//Responds with the friend names of the user provided
app.get('/getFriends', function (req, res) {
  var sql = `SELECT username FROM Users WHERE id IN (SELECT f1.receiver FROM Friends f1 LEFT JOIN Friends f2 ON f1.sender = f2.receiver AND f2.sender = f1.receiver WHERE f1.sender = ${req.userId} AND f2.sender IS NOT NULL);`;
  con.query(sql, function(err, result, fields){
    if(err) {
      res.status(404).end();}
    else{
      var toSend = [];
      result.forEach(element => {
          toSend.push(element.username);
      });
      if (toSend.length == 0) toSend = "none";
      res.status(200).send(JSON.stringify(toSend));
    }
  })
})

//Responds with the names of the users who requested to be friends with the user provided
app.get('/getFriendsRequests', function (req, res) {
  var sql = `SELECT username FROM Users WHERE id IN (SELECT f1.sender FROM Friends f1 LEFT JOIN Friends f2 ON f1.sender = f2.receiver AND f2.sender = f1.receiver WHERE f1.receiver = ${req.userId} AND f2.sender IS NULL);`;
  con.query(sql, function(err, result, fields){
    if(err) {
      res.status(404).end();}
    else{
      var toSend = [];
      result.forEach(element => {
          toSend.push(element.username);
      });
      if (toSend.length == 0) toSend = "none";
      console.log(JSON.stringify(toSend));
      res.status(200).send(JSON.stringify(toSend));
    }
  })
})

//Responds with the names and coins of the friends of the user provided
app.get('/getFriendsData', function (req, res) {
  var sql = `SELECT username,coins FROM Users WHERE id IN (SELECT f1.receiver FROM Friends f1 LEFT JOIN Friends f2 ON f1.sender = f2.receiver AND f2.sender = f1.receiver WHERE f1.sender = ${req.userId} AND f2.sender IS NOT NULL) ORDER BY coins DESC;`;
  con.query(sql, function(err, result, fields){
    if(err) {
      res.status(404).end();}
    else{
      var toSend = [];
      result.forEach(e => {
          element = {"username":e.username, "coins":e.coins};
          toSend.push(element);
      });
      if (result.length == 0) result = "none";
      console.log(JSON.stringify(toSend));
      res.status(200).send(JSON.stringify(toSend));
    }
  })
})

//Inserts a friend request from the provided user to the user in the request's body
app.post('/addFriend',function (req, res){
  var friendname = req.body.name;
  var sql = `INSERT INTO Friends VALUES (${req.userId}, (SELECT id FROM Users WHERE username = "${friendname}"));`;
  con.query(sql, function(err, result, fields){
    if(err) {
      res.status(404).end();
    }else{
      res.status(200).send();
    }
  })
})

//Inserts a friend request from the provided user to the user in the request's body
app.post('/acceptFriend',function (req, res){
  var friendname = req.body.name;
  var sql = `INSERT INTO Friends VALUES (${req.userId}, (SELECT id FROM Users WHERE username = "${friendname}"));`;
  con.query(sql, function(err, result, fields){
    if(err) {
      res.status(404).end();
    }else{
      res.status(200).send();
    }
  })
})

//Removes the friend request made to the provided user from the usern on the request's body
app.post('/rejectFriend',function (req, res){
  var friendname = req.body.name;
  var sql = `DELETE FROM Friends WHERE receiver = ${req.userId} AND sender = (SELECT id FROM Users WHERE username = "${friendname}"));`;
  con.query(sql, function(err, result, fields){
    if(err) {
      res.status(404).end();
    }else{
      res.status(200).send();
    }
  })
})

//Removes the both friends request between the user and the user on the request's body
app.delete('/deleteFriend', function (req, res) {
  var friendname = req.body;
  var sql = `DELETE From Friends WHERE (sender = "${req.userId}" AND receiver = "${friendname}") OR (sender = "${friendname}" AND receiver = "${req.userId}")`;
  con.query(sql, function(err, result, fields){
    if(err) {
      res.end(JSON.stringify(result));}
    else{
      res.end(JSON.stringify(false));
    }
  })
})

//====================[ Bets Module ]=========================
//Responds with all the bets of the user
app.get('/getBets', function (req, res) {
  var sql = `SELECT * FROM Bets WHERE username = "${req.user}"`;
  con.query(sql, function(err, result, fields){
    if(err) {
      res.status(404).end();
    }else{
      res.end(JSON.stringify(result));
    }
  })
})

//Responds with the name of the subjects studied by the user provided as a parameter in which the user can bet
app.get('/getPosibleBets/:username', function (req, res) {
  var sql = `SELECT s.studentname, s.subjectname, m.studentname, b.studentname FROM Studying s LEFT JOIN Bets b ON s.studentname = "${req.params.name}" AND s.studentname = b.studentname LEFT JOIN Marks m ON s.studentname = m.studentname AND s.subjectname = m.subjectname WHERE s.studentname = "${req.params.name}" AND m.markObtained is NULL AND b.markExpected is NULL;`;
  con.query(sql, function(err, result, fields){
    if(err) {
      res.status(404).end();
    }else{
      var toSend = [];
      result.forEach(element => {
          toSend.push(element.subjectname);
      });
      res.status(200).end(JSON.stringify(toSend));
    }
  })
})

//Inserts a new bet with the data provided if the user has enough coins to do so, then substracts the amount of coins
app.post('/addBet',function (req, res){
  var bet = req.body;
  con.query(`SELECT coins FROM Users WHERE id = ${req.userId}`, function(err, result){
    if(result[0].coins < bet.coins){
      res.status(403).end();
    }else{
      var sql = `INSERT INTO Bets VALUES ("${bet.username}", "${bet.studentname}", "${bet.subjectname}", ${bet.coins}, ${bet.markExpected})`;
      con.query(sql, function(err, result){
        if(err) {
          res.status(400).end();
        }else{
          con.query(`UPDATE Users SET coins = coins - ${bet.coins} WHERE id = ${req.userId}`, function(err, result){
            if(err) {
              res.status(400).end();
            }else{
              res.status(200).end();
            }
          })
        }
      });
    }
  });
})


//===================[ Listener del puerto 23415 ]================
//Instantiate the server to run
var server = app.listen(config.appPort, function () {
   var host = server.address().address
   var port = server.address().port
   console.log(`PINFbet: Escuchando peticiones en el puerto: ${port}`)
})