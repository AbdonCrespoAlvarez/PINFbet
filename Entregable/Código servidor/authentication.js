var jwt = require('jwt-simple');
var moment = require('moment');
var config = require('./config');

// Subobjectives: SO_4, SO_9.

//Function which creates an authorization and authentication token which allows the user
//to send request in the future. The token can expire and also holds the name and id of the
//provided user
exports.createToken = function(username, userid) {
    var tokenInfo = {
        user: username,
        idUser: userid,
        iat: moment().unix(),
        exp: moment().add(1, "days").unix(),
    };
    return jwt.encode(tokenInfo, config.TOKEN_CODE);
};