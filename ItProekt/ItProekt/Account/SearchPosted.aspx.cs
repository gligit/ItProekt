using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Web.Configuration;
using System.Runtime.Serialization;

namespace ItProekt
{
    public partial class WebForm3 : System.Web.UI.Page
    {
        protected void Page_Init(object sender, EventArgs e)
        {
            string UID = App_Code.Class1.GetUserIDFromUserName(Label1);
            if (UID != null)
            {
                App_Code.Class1.GetAllStickersForUsedID(UID,ListBox1,Label1);
                if (Request.QueryString["search"] != null)
                {
                    LoadSearch((string)Request.QueryString["search"]);
                    Button5.Visible = true;
                    Button6.Visible = true;
                }
                else
                {
                    LoadSearch("");
                    Button5.Visible = true;
                    Button6.Visible = true;
                }

            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            Label1.Text = "";
            Label2.Text = "";
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
            }
            else
            {
                Image1.ImageUrl = "";
            }
        }

        //protected void Button1_Click(object sender, EventArgs e)
        //{
        //    Session["Show"] = TextBox1.Text;
        //    Response.Redirect(Request.RawUrl);
        //}

        private void LoadSearch(string searchfor)
        {
            SqlConnection con = null;
            try
            {

                bool should_filter = false;
                if (!string.IsNullOrWhiteSpace(searchfor))
                {
                    searchfor.ToLower();
                    should_filter = true;
                }
                con = new SqlConnection(WebConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);

                string c = "SELECT PostSticker.PosterID,PostSticker.StickerID ,PostSticker.Price, PostSticker.AcceptsTrading, PostSticker.Date, Sticker.PlayerName, Sticker.Extension, aspnet_Users.UserName FROM PostSticker INNER JOIN Sticker ON PostSticker.StickerID=Sticker.StickerID INNER JOIN aspnet_Users ON aspnet_Users.UserID=Sticker.UserID";
                SqlCommand command = new SqlCommand(c, con);

                con.Open();
                SqlDataReader reader = command.ExecuteReader();

                List<StickerImage> stickers = new List<StickerImage>();

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
                        int l = searchfor.Length;

                        if (PlayerName.Contains(searchfor))
                            found = true;
                        else if (PlayerName.Contains(searchfor.Substring(0, l / 2)))
                            found = true;
                        else if (PlayerName.Contains(searchfor.Substring(l / 2, l / 2)))
                            found = true;
                        else
                            found = false;
                    }

                    if (found)
                    {
                        stickers.Add(new StickerImage(reader["StickerID"].ToString(),
                        reader["Extension"].ToString(), reader["PlayerName"].ToString(),
                        reader["Date"].ToString(), (string)reader["UserName"],
                        reader["PosterID"].ToString(), (int)reader["Price"],
                        reader["AcceptsTrading"].ToString(), Label3, Image2));
                    }
                }

                ////////////////////////////////////////////////////////////////

                View v1 = new View();
                Panel p = new Panel();
                p.BorderColor = System.Drawing.Color.FromArgb(155, 111, 20);
                p.BorderStyle = BorderStyle.Outset;
                p.BorderWidth = 10;


                for (int i = 0; i < stickers.Count; i++)
                {
                    p.Controls.Add(stickers[i]);

                    if ((i % 19 == 0 && i != 0) || (i + 1 == stickers.Count))
                    {
                        v1.Controls.Add(p);
                        MultiView1.Controls.Add(v1);
                        v1 = new View();
                        p = new Panel();
                        p.BorderColor = System.Drawing.Color.FromArgb(155, 111, 20);
                        p.BorderStyle = BorderStyle.Outset;
                        p.BorderWidth = 10;
                    }
                }

                if (stickers.Count != 0)
                {
                    if (MultiView1.Controls.Count != 0)
                        MultiView1.ActiveViewIndex = 0;
                    else
                        MultiView1.ActiveViewIndex = -1;
                }
                else
                {
                    MultiView1.ActiveViewIndex = -1;
                    Image2.ImageUrl = "";
                    Label3.Text = "";
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

        protected void Button3_Click(object sender, EventArgs e)
        {
            if (Session["Selected"] != null)
            {
                StickerImage s = ((StickerImage)Session["Selected"]);
                string ThisUserID = App_Code.Class1.GetUserIDFromUserName(Label2);
                if (ThisUserID != s.PosterID)
                {
                    if (!s.AcceptsTrading)
                    {
                        int offer = 0;
                        if (int.TryParse(TextBox3.Text, out offer))
                        {
                            App_Code.Class1.CreateMessageSticker(s.StickerID, null, s.PosterID, ThisUserID, offer, true, Label2);
                        }
                        else
                        {
                            Label2.Text = "Корисникот прифаќа само пари, внесете понуда";
                        }
                    }
                    else
                    {
                        int q = 0;
                        if (ListBox1.SelectedIndex != -1 || int.TryParse(TextBox3.Text, out q))
                        {
                            int.TryParse(TextBox3.Text, out q);
                            string SenderStickerID = null;
                            if (ListBox1.SelectedIndex != -1)
                            {
                                SenderStickerID = ListBox1.SelectedItem.Value.Split(' ')[0];

                            }
                            App_Code.Class1.CreateMessageSticker(s.StickerID, SenderStickerID, s.PosterID, ThisUserID, q, true, Label2);
                        }
                        else
                        {
                            Label2.Text = "Морате да понудите цена или пак да селектирате сликичка за менување";
                        }
                    }
                }
                else
                {
                    Label2.Text = "Тоа е вашата сликичка";
                }

            }
            else
            {
                Label2.Text = "Немате селектирано сликичка";
            }
        }


        protected void Button4_Click(object sender, EventArgs e)
        {
            ListBox1.SelectedIndex = -1;
            PlayerInfo.Text = "";
            Image1.ImageUrl = "";
        }

        protected void Button5_Click(object sender, EventArgs e)
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

        protected void Button6_Click(object sender, EventArgs e)
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


    }

    [Serializable]
    class StickerImage : ImageButton, ISerializable
    {
        public Label l { get; set; }
        public string StickerID { get; set; }
        public string PlayerName { get; set; }
        public string PostDate { get; set; }

        public string UserName { get; set; }
        public string PosterID { get; set; }
        public int Price { get; set; }
        public bool AcceptsTrading;
        Image image;

        private StickerImage(SerializationInfo info, StreamingContext context)
            : base()
        {
            StickerID = info.GetString("StickerID");
            PlayerName = info.GetString("PlayerName");
            PostDate = info.GetString("PostDate");
            UserName = info.GetString("UserName");
            PosterID = info.GetString("PosterID");
            Price = info.GetInt32("Price");
            AcceptsTrading = info.GetBoolean("AcceptsTrading");
            l.Text += "calling from private consturctoer";
        }
        public void GetObjectData(SerializationInfo inf, StreamingContext con)
        {
            inf.AddValue("StickerID", StickerID);
            inf.AddValue("PlayerName", PlayerName);
            inf.AddValue("PostDate", PostDate);
            inf.AddValue("UserName", UserName);
            inf.AddValue("PosterID", PosterID);
            inf.AddValue("Price", Price);
            inf.AddValue("AcceptsTrading", AcceptsTrading);
        }
        public StickerImage(string StickerID, string Extension, string PlayerName, string PostDate,
            string UserName, string PosterID, int Price, string AcceptsTrading, Label l, Image image)
        {
            this.l = l;
            this.StickerID = StickerID;
            this.PlayerName = PlayerName;
            this.PostDate = PostDate.ToString();

            this.UserName = UserName;
            this.PosterID = PosterID;
            this.Price = Price;
            if (AcceptsTrading == "True")
                this.AcceptsTrading = true;
            else
                this.AcceptsTrading = false;

            this.image = image;
            string RelativePath = @"~\Images\" + StickerID + Extension;
            base.ImageUrl = RelativePath;
            base.Click += new ImageClickEventHandler(StickerImage_Click);
            base.CssClass = "Stickers";

        }

        void StickerImage_Click(object sender, ImageClickEventArgs e)
        {
            HttpContext.Current.Session["Selected"] = this;

            image.ImageUrl = base.ImageUrl;
            l.Text = "Име на играчот: " + PlayerName +
     "<br/>Сликичката е постирана на " + PostDate + " од " + UserName +
      "<br/>ID на сликичката " + StickerID + "<br/>Цена " + Price +
      "<br/>Прифаќа замена " + AcceptsTrading;
        }

    }
}