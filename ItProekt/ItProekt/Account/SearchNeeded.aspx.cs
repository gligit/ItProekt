using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Web.Configuration;

namespace ItProekt
{
    public partial class WebForm4 : System.Web.UI.Page
    {
        protected void Page_Init(object sender, EventArgs e)
        {
             string UID = App_Code.Class1.GetUserIDFromUserName(Label1);
             App_Code.Class1.GetAllStickersForUsedID(UID, ListBox1, Label1);
              
                 Button2.Visible = true;
                 Button3.Visible = true;
                 show();
               
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Label1.Text = "";
            Label2.Text = "";
            StatusMessage.Text = "";
        }
        protected void Button1_Click(object sender, EventArgs e)
        {
           
        }

        private void show()
        {
            SqlConnection con = null;
            try
            {
                string LookingFor = "";
                if (Request.QueryString["search"] != null)
                    LookingFor = (string)Request.QueryString["search"];

                bool should_filter = false;
                if (!string.IsNullOrWhiteSpace(LookingFor))
                {
                    LookingFor.ToLower();
                    should_filter = true;
                }
                con = new SqlConnection(WebConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);

                string c = "SELECT aspnet_Users.UserName,NeedSticker.UserID,NeedSticker.PlayerName,NeedSticker.Date FROM NeedSticker INNER JOIN aspnet_Users ON NeedSticker.UserID = aspnet_Users.UserID";
                SqlCommand command = new SqlCommand(c, con);

                con.Open();
                SqlDataReader reader = command.ExecuteReader();

                List<NeededSticker> stickers = new List<NeededSticker>();

                while (reader.Read())
                {
                    bool found = false;
                    if (!should_filter)
                    {
                        found = true;
                    }
                    else
                    {
                        string PlayerName = reader["PlayerName"].ToString().ToLower();
                        int l = LookingFor.Length;

                        if (PlayerName.Contains(LookingFor))
                            found = true;
                        else if (PlayerName.Contains(LookingFor.Substring(0, l / 2)))
                            found = true;
                        else if (PlayerName.Contains(LookingFor.Substring(l / 2, l / 2)))
                            found = true;
                        else
                            found = false;
                    }

                    if (found)
                    {
                        stickers.Add(new NeededSticker(reader["UserID"].ToString(), reader["UserName"].ToString(), reader["PlayerName"].ToString(), reader["Date"].ToString()));
                    }
                }
                Session["NeedSticker"] = stickers;
                //////////////

                View v1 = new View();

                for (int i = 0; i < stickers.Count; i++)
                {

                    Panel p = new Panel();
                    Label space = new Label();
                    space.Text = " ";
                    Button b = new Button();
                    b.Text = "Селектирај";
                    b.Click += new EventHandler(b_Click);
                    stickers[i].b = b;

                    p.Controls.Add(stickers[i]);
                    p.Controls.Add(space);
                    p.Controls.Add(b);

                    v1.Controls.Add(p);

                    if ((i % 19 == 0 && i != 0) || (i + 1 == stickers.Count))
                    {
                        MultiView1.Controls.Add(v1);
                        v1 = new View();
                    }
                }
                if (stickers.Count != 0)
                {
                    if (MultiView1.Controls.Count != 0)
                        MultiView1.ActiveViewIndex = 0;
                    else
                        MultiView1.ActiveViewIndex = -1;

                }
            }
            catch (Exception err)
            {
                Label1.Text = "Грешка (4)" + err.Message;
            }
            finally
            {
                if (con != null)
                    con.Close();
            }

         
        }
        void b_Click(object sender, EventArgs e)
        {
            List<NeededSticker> stickers = (List<NeededSticker>)Session["NeedSticker"];

            for (int i = 0; i < stickers.Count; i++)
            {
                if (stickers[i].b == sender)
                {
                    Session["SelectedNeedSticker"] = stickers[i];
                    SelectedPost.Text = "Селектиран оглас: " + stickers[i].PlayerName + " од " + stickers[i].UserName;
                    break;
                }
            }
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            //left
            if (MultiView1.Controls.Count == 0)
                MultiView1.ActiveViewIndex = -1;
            else
            {
              int index = MultiView1.ActiveViewIndex - 1;
              if (index < 0)
                  MultiView1.ActiveViewIndex = 0;
              else
                  MultiView1.ActiveViewIndex = index;
            }
        }

        protected void Button3_Click(object sender, EventArgs e)
        {
            //rightbutton
            int max = MultiView1.Controls.Count - 1;
            int index = MultiView1.ActiveViewIndex + 1;

            if (MultiView1.Controls.Count == 0)
                MultiView1.ActiveViewIndex = -1;
            else if (index > max)
                MultiView1.ActiveViewIndex = max;
            else
                MultiView1.ActiveViewIndex = index;
        }

        protected void ListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ListBox1.SelectedIndex != -1)
            {
                string[] pl = ListBox1.SelectedItem.Value.Split(' ');
                string StickerID = pl[0];
                string Date = pl[1];
                Image1.ImageUrl = pl[3];

                PlayerInfo.Text = "Име на играчот: " + ListBox1.SelectedItem.Text +
                    "<br/>Сликичката е креирана на " + Date + " " + pl[2] +
                     "<br/>ID на сликичката " + StickerID;

                Image1.Visible = true;
            }
            else
            {
                Image1.Visible = false;
            }
        }

        protected void Button4_Click(object sender, EventArgs e)
        {
            if (Session["SelectedNeedSticker"] != null)
            {
                NeededSticker n = (NeededSticker)Session["SelectedNeedSticker"];
                string ThisUserdID = App_Code.Class1.GetUserIDFromUserName(Label1);

                if (ThisUserdID != n.UserID)
                {
                    if (ListBox1.SelectedIndex != -1)
                    {
                        int Price = 0;
                        if (int.TryParse(TextBox2.Text, out Price))
                        {

                            string SenderStickerID = ListBox1.SelectedItem.Value.ToString().Split(' ')[0];

                            App_Code.Class1.CreateMessageSticker(null, SenderStickerID, n.UserID, ThisUserdID, Price, false, StatusMessage);
                        }
                        else
                        {
                            StatusMessage.Text = "Внесе ја цената за вашата сликичка";
                        }
                    }
                    else
                    {
                        StatusMessage.Text = "Селектирајте сликичка";
                    }
                }
                else
                {
                    StatusMessage.Text = "Тоа е ваш оглас";
                }
            }
            else
            {
                StatusMessage.Text = "Селектирајте оглас";
            }
        }
    }
    class NeededSticker : Label
    {
        public string UserID { get; set; }
        public string UserName { get; set; }
        public string PlayerName { get; set; }
        public string Date { get; set; }
        public Button b { get; set; }
        public NeededSticker(string UserID,string UserName, string PlayerName, string Date)
        {
            this.UserID = UserID;
            this.UserName = UserName;
            this.PlayerName = PlayerName;
            this.Date = Date;
            this.b = null;

            base.ForeColor = System.Drawing.Color.FromName("white");
            base.Font.Size = FontUnit.Large;
        
            base.Text = UserName + " го поставил огласот за играчот " + PlayerName + " " + "на "+ Date;
        }
    }
}