using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Web.Configuration;
using System.IO;
using System.Drawing;

namespace ItProekt
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            if (App_Code.Class1.CheckPlayerName(TextBox4, Label1) && CheckFile())
            {
                SqlConnection con = null;
                try
                {
                    con = new SqlConnection(WebConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);

                    SqlCommand command = new SqlCommand("CheckIfUserExists", con);
                    command.Parameters.Add(new SqlParameter("@UserName", TextBox1.Text));
                    command.CommandType = System.Data.CommandType.StoredProcedure;

                    command.Parameters.Add("@UserOut", System.Data.SqlDbType.UniqueIdentifier).Direction = System.Data.ParameterDirection.Output;

                    con.Open();
                    command.ExecuteNonQuery();

                    if (command.Parameters["@UserOut"].Value != System.DBNull.Value)
                    {
                        string uid = command.Parameters["@UserOut"].Value.ToString();
                        MakeSticker(uid);
                    }
                    else
                    {
                        Label1.Text = "Не постои корисник со такво Корисничко име";
                    }
                }
                catch (Exception)
                {
                    Label1.Text = "Грешка, обидете се повторно (1)";
                }
                finally
                {
                    if (con != null)
                        con.Close();
                }
            }
        }


        private string MakeSticker(string uid)
        {
            SqlConnection con = null;
            try
            {
                con = new SqlConnection(WebConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
                SqlCommand command = new SqlCommand("MakeAndReturnStickerID", con);
                command.Parameters.Add(new SqlParameter("@UserID", uid));
                command.Parameters.Add(new SqlParameter("@PlayerName", TextBox4.Text));
                command.Parameters.Add("@StickerID", System.Data.SqlDbType.UniqueIdentifier).Direction = System.Data.ParameterDirection.Output;

                string extension = Path.GetExtension(FileUpload1.FileName);
                command.Parameters.AddWithValue("@Extension", extension);

                command.CommandType = System.Data.CommandType.StoredProcedure;
                con.Open();
                command.ExecuteNonQuery();

                if (command.Parameters["@StickerID"].Value != System.DBNull.Value)
                {
                    string name = command.Parameters["@StickerID"].Value.ToString();
                    string relativePath = @"~\Images\" + name + extension;

                    Stream s = FileUpload1.PostedFile.InputStream;
                    Bitmap b = new Bitmap(s);
                    Bitmap b1 = new Bitmap(b, 120, 120);
                    b1.Save(Page.Server.MapPath(relativePath));

                    Label1.Text = "Успешно креирана сликичка";
                    return name;
                }
                else
                {
                    Label1.Text = "Неуспешно креирање сликичка (2)";
                }
            }
            catch (Exception err)
            {
                Label1.Text = "Неуспешно креирање сликичка (1)" + err.Message;
            }
            finally
            {
                if (con != null)
                    con.Close();
            }
            return null;
        }

        protected void Button3_Click(object sender, EventArgs e)
        {
            int price = 0;
            if (App_Code.Class1.CheckPlayerName(TextBox4, Label1) && CheckFile() && App_Code.Class1.CheckForNumber(TextBox3.Text, out price, Label1))
            {
                string uid = App_Code.Class1.GetUserIDFromUserName(Label1);

                if (uid != null)
                {
                    string StickerID = MakeSticker(uid);

                    if (StickerID != null)
                    {
                        App_Code.Class1.PostSticker(Label1, StickerID, uid, price, false);
                    }
                }

            }
        }


        private bool CheckFile()
        {
            if (FileUpload1.HasFile)
            {
                if (!FileUpload1.PostedFile.ContentType.StartsWith("image"))
                {
                    Label1.Text = "Датотеката не е слика";
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                Label1.Text = "Нема селектирано слика";
                return false;
            }
        }

    }
}