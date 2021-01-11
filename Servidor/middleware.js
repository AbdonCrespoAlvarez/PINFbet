var jwt = require('jwt-simple');
var moment = require('moment');
var config = require('./config');

//Middleware function which only allows for requests which holds an authorization token
//and such token is valid. Then the user and userId fields are defined on the request with
//the name and id of the provided token data
exports.ensureAuthenticated = function(req, res, next){
    if(!req.headers.authorization){
        console.log("Authorization Failed");
        return res
        .status(403)
        .send({message: "No authorization header"});
    }

    var token = req.headers.authorization;
    var tokenInfo = jwt.decode(token, config.TOKEN_CODE);

    if(tokenInfo.exp <= moment().unix()){
        console.log("Authorization Failed");
        return res
            .status(401)
            .send({message: "Token expired"});
    }
    req.user = tokenInfo.user;
    req.userId = tokenInfo.idUser;
    next();
}

//Middleware function which logs some useful information on the console about any requests received
exports.info = function(req, res, next){
    console.log('<'+req.user+">("+req.userId+")===> " + req.method + ": "+ req.protocol + '://' + req.get("host") + req.originalUrl);
    next();
}