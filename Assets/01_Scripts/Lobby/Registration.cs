using System;
using UnityEngine;
using UnityEngine.UI;
using MongoDB.Driver;
using Zoo.Web;
using Zoo.Network;
using Zoo.Core;

namespace Zoo.Lobby
{
    public class Registration : MonoBehaviour
    {
        [SerializeField] private GameObject player, customization, dynamicCanvas;

        [SerializeField] private Text errorRegister, errorLogin;

        [SerializeField] private Texture2D mouseCursor;

        // Use this for initialization
        void Start()
        {
            Cursor.SetCursor(mouseCursor, Vector2.zero, CursorMode.ForceSoftware);
        }

        public void RegisterUser(Transform register)
        {
            string username = register.Find("username").GetChild(0).GetComponent<InputField>().text;
            string password = register.Find("password").GetChild(0).GetComponent<InputField>().text;
            string email = register.Find("email").GetChild(0).GetComponent<InputField>().text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(email))
            {
                errorRegister.gameObject.SetActive(true);
                errorRegister.text = "You did not fill out the required fields";
                errorRegister.color = UnityEngine.Color.red;
            }
            else
            {
                if (!Utility.IsMail(email))
                {
                    errorRegister.gameObject.SetActive(true);
                    errorRegister.text = "The email field is not a valid e-mail address";
                    errorRegister.color = UnityEngine.Color.red;
                }
                else
                {
                    if (WebManager.singleton.db.IsUsernameExisted<User>(username))
                    {
                        errorRegister.gameObject.SetActive(true);
                        errorRegister.text = "The username has already been taken";
                        errorRegister.color = UnityEngine.Color.red;
                    }
                    else if (WebManager.singleton.db.IsEmailExisted<User>(email))
                    {
                        errorRegister.gameObject.SetActive(true);
                        errorRegister.text = "The email has already been taken";
                        errorRegister.color = UnityEngine.Color.red;
                    }
                    else
                    {
                        register.Find("registerBtn").GetComponent<Button>().interactable = false;
                        User user = new User();
                        user.username = username;
                        user.password = Zoo.Core.Utility.Sha256FromString(password);
                        user.email = email;
                        WebManager.singleton.db.Insert<User>("rpg_users", user, OnRegisterUser);
                    }
                }
            }
        }

        private void OnRegisterUser(bool success)
        {
            Transform register = transform.Find("Register");

            if (success)
            {
                register.Find("username").GetChild(0).GetComponent<InputField>().text = "";
                register.Find("password").GetChild(0).GetComponent<InputField>().text = "";
                register.Find("email").GetChild(0).GetComponent<InputField>().text = "";
                register.Find("registerBtn").GetComponent<Button>().interactable = true;
                errorRegister.gameObject.SetActive(true);
                errorRegister.text = "Your account has been successfully created";
                errorRegister.color = UnityEngine.Color.black;
            }
            else
            {
                register.Find("registerBtn").GetComponent<Button>().interactable = true;
                Debug.Log("Somenthing wrong creating new user!!");
            }
        }

        public void OnRegisterChanged()
        {
            errorRegister.gameObject.SetActive(false);
            errorRegister.text = "";
        }

        public void LoginUser(Transform login)
        {
            string username = login.Find("username").GetChild(0).GetComponent<InputField>().text;
            string password = login.Find("password").GetChild(0).GetComponent<InputField>().text;
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                errorLogin.gameObject.SetActive(true);
                errorLogin.text = "You did not fill out the required fields";
            }
            else
            {
                login.Find("loginBtn").GetComponent<Button>().interactable = false;
                FilterDefinition<User> filterOr = Builders<User>.Filter.Or(Builders<User>.Filter.Eq("email", username), Builders<User>.Filter.Eq("username", username));
                FilterDefinition<User> filter = Builders<User>.Filter.And(filterOr, Builders<User>.Filter.Eq("password", Zoo.Core.Utility.Sha256FromString(password)));
                WebManager.singleton.db.Select<User>("rpg_users", filter, OnLoginUser);
            }
        }

        private void OnLoginUser(bool success, User user)
        {
            Transform login = transform.Find("Login");

            if (success)
            {
                string token = Zoo.Core.Utility.GenerateRandom(60);
                DateTime now = DateTime.Now;
                user.token = token;
                user.lastLogin = now;
                UpdateDefinition<User> update = Builders<User>.Update
                    .Set(it => it.token, token)
                    .Set(it => it.lastLogin, now);
                WebManager.singleton.db.Update<User>("rpg_users", user._id, update);
                MultiplayerManager.user = user;
                login.Find("loginBtn").GetComponent<Button>().interactable = true;
                login.Find("username").GetChild(0).GetComponent<InputField>().text = "";
                login.Find("password").GetChild(0).GetComponent<InputField>().text = "";

                FilterDefinition<Character> filter = Builders<Character>.Filter.Eq("user", user);
                WebManager.singleton.db.Select<Character>("rpg_characters", filter, OnCharacter);
            }
            else
            {
                login.Find("loginBtn").GetComponent<Button>().interactable = true;
                errorLogin.gameObject.SetActive(true);
                errorLogin.text = "Your username/email or password is incorrect";
                Debug.Log("Somenthing wrong login your account!!");
            }
        }

        private void OnCharacter(bool success, Character character)
        {
            gameObject.SetActive(false);

            if(success)
            {
                Debug.Log("Opened selecter");
                player.SetActive(true);
            }
            else
            {
                Debug.Log("Opened customization");
                customization.SetActive(true);
                dynamicCanvas.SetActive(true);
            }
        }

        public void OnLoginChanged()
        {
            errorLogin.gameObject.SetActive(false);
            errorLogin.text = "";
        }
    }
}