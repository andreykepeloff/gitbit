﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace vk_bot
{
    public partial class Form1 : Form
    {
        string access_token;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            webBrowser1.Dock = DockStyle.Fill;
            webBrowser1.Navigate("https://oauth.vk.com/authorize?client_id=6410347&redirect_uri=https://oauth.vk.com/blank.html&scope=friends+messages&response_type=token&v=5.73");
       
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            string url = e.Url.ToString();
            if (url.Contains("error"))
            {
                MessageBox.Show("Ошибка");
            }
            if (url.Contains("access_token"))
            {
                int IndexAccTok = url.IndexOf("access_token");
                access_token = url.Remove(0, IndexAccTok + 13);
                int IndexAmp = access_token.IndexOf("&");
                access_token = access_token.Remove(IndexAmp);

                XmlDocument doc = new XmlDocument();
                doc.Load("https://api.vk.com/method/users.get.xml?fields=photo_100&access_token=" + access_token+"&v=5.73");
                XmlNode response = doc.SelectSingleNode("response");
                XmlNode user = response.SelectSingleNode("user");

                XmlNode FirstName = user.SelectSingleNode("first_name");
                labelFirstName.Text = FirstName.InnerText;
             
                XmlNode LastName = user.SelectSingleNode("last_name");
                labelLastName.Text = LastName.InnerText;

                pictureBoxAvatar.ImageLocation = user.SelectSingleNode("photo_100").InnerText;
                webBrowser1.Visible = false;
            }

        }

        private void ya_Click(object sender, EventArgs e)
        {
            FormYa frm = new FormYa();
            frm.Access_token = access_token;
            frm.Show();
        }

        private void deletefriends_Click(object sender, EventArgs e)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("https://api.vk.com/method/friends.get.xml?fields=photo_200_orig&access_token=" + access_token+"&v=5.73");
            XmlNode response = doc.SelectSingleNode("response");
            XmlNode items= response.SelectSingleNode("items");
            foreach (XmlNode user in items.SelectNodes("user"))
            {
                XmlNode photo = user.SelectSingleNode("photo_200_orig");
                if (photo.InnerXml.Contains("deactivated_200"))
                {
                    XmlDocument document = new XmlDocument();
                    document.Load("https://api.vk.com/method/friends.delete.xml?user_id=" + user.SelectSingleNode("id").InnerText+ "&access_token=" + access_token + "&v=5.73");
                    XmlNode id = document.SelectSingleNode("id");
                    System.Threading.Thread.Sleep(100);
                    labelFriendsDelete.Text = "All Friends Delete";
                }
            }
                
        }

        private void DeleteComments_Click(object sender, EventArgs e)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("https://api.vk.com/method/wall.get.xml?owner_id=315032483&access_token=" + access_token + "&v=5.73");
            XmlDocument docs = new XmlDocument();
            docs.Load("https://api.vk.com/method/wall.getComment.xml?owner_id=315032483&access_token=" + access_token + "&v=5.73");
            XmlNode response = doc.SelectSingleNode("response");
            XmlNode items = response.SelectSingleNode("items");
            foreach (XmlNode post in items.SelectNodes("post"))
            {
                XmlNode censcom = post.SelectSingleNode("post");
                if (censcom.InnerXml.Contains(""))
                {
                    XmlDocument document = new XmlDocument();
                    document.Load("https://api.vk.com/method/wall.deleteComment.xml?owner_id=315032483&post_id=" + post.SelectSingleNode("id").InnerText + "&access_token=" + access_token + "&v=5.73");
                    XmlNode id = censcom.SelectSingleNode("id");
                    labelCensure.Text = "All Comments Delete";
                }
            }
        }
    }
}
