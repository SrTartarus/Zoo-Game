﻿using System;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace Zoo.Core
{
    public class Utility
    {

        public const string EMAIL_PATTERN = @"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*@((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))\z";
        public const string USERNAME_AND_DISCRIMINATOR_PATTERN = @" ^[a-zA-Z0-9]{4,20}#[0-9]{4}$";
        public const string USERNAME_PATTERN = @"^[a-zA-Z0-9]{4,20}$";
        public const string RANDOM_CHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        public static bool IsMail(string email)
        {
            if (email != null)
                return Regex.IsMatch(email, EMAIL_PATTERN);
            else
                return false;
        }

        public static bool IsUsername(string username)
        {
            if (username != null)
                return Regex.IsMatch(username, USERNAME_PATTERN);
            else
                return false;
        }

        public static bool IsUsernameAndDiscriminator(string username)
        {
            if (username != null)
                return Regex.IsMatch(username, USERNAME_AND_DISCRIMINATOR_PATTERN);
            else
                return false;
        }

        public static string GenerateRandom(int length)
        {
            Random r = new Random();
            return new string(Enumerable.Repeat(RANDOM_CHARS, length).Select(s => s[r.Next(s.Length)]).ToArray());
        }

        public static string Sha256FromString(string toEncrypt)
        {
            var message = Encoding.UTF8.GetBytes(toEncrypt);
            SHA256Managed hashString = new SHA256Managed();

            string hex = "";
            var hashValue = hashString.ComputeHash(message);
            foreach (byte x in hashValue)
            {
                hex += String.Format("{0:x2}", x);
            }

            return hex;
        }

        public static float Denormalize(float normalize, float max, float min)
        {
            return normalize * (max - min) + min;
        }

        public static float Normalize(float value, float max, float min)
        {
            return (value - min) / (max - min);
        }
    }
}

