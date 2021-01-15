using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Security.Cryptography;
using System.Text;
using System;

public static class Encrypter
{

    /*
    Subobjectives: SO_4.
    Returns an encrypted version of the input based on a given salt and the SHA256 algorithm.
    */
    public static string generateHash(string input, string salt)
    {
        byte[] passwordBytes = Encoding.ASCII.GetBytes(input);
        byte[] saltBytes = Encoding.ASCII.GetBytes(salt);
        HashAlgorithm encrypter = new SHA256Managed();
        byte[] plainTextWithSaltBytes = new byte[passwordBytes.Length + saltBytes.Length];

        for (int i = 0; i < passwordBytes.Length; i++)
            plainTextWithSaltBytes[i] = passwordBytes[i];

        for (int i = 0; i < saltBytes.Length; i++)
            plainTextWithSaltBytes[passwordBytes.Length + i] = saltBytes[i];

        return Convert.ToBase64String(encrypter.ComputeHash(plainTextWithSaltBytes));
    }

}
