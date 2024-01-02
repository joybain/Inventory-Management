using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Delve;
using System.Data.SqlClient;
using System.Data;

public partial class frmImage : System.Web.UI.Page
{
    SqlConnection connection = new SqlConnection(DataManager.OraConnString());
    DataTable dddt;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            //getImage();
            if (Request.QueryString["ID"].ToString() == "")
            {
                BindGridData();
                //DataTable dddt = (DataTable)Session["Img"];
            }
            else
            {
                IdManager.getInsertUpdateDelete("DELETE FROM [TemporaryImage]");
                if (Request.QueryString["RefNo"].ToString() == "00")
                {
                    dddt = IdManager.GetShowDataTable("SELECT [ID],[Image] FROM [Temp_ShiftmentItemsImage] WHERE [MasterId] ='" + Request.QueryString["ID"].ToString() + "' ");
                }
                else
                {                   
                    dddt = IdManager.GetShowDataTable("SELECT [ID],[Image] FROM [ShiftmentBoxingItemsImage] WHERE [BoxingItemsID] ='" + Request.QueryString["ID"].ToString() + "' and ItemsID='" + Request.QueryString["RefNo"].ToString() + "' ");
                }
                foreach (DataRow dr in dddt.Rows)
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand("INSERT INTO TemporaryImage (ImageID,Image) VALUES (@ImageID,@imagedata)", connection);
                    cmd.Parameters.Add("@ImageID", SqlDbType.Int, 50).Value = dr["ID"].ToString();
                    cmd.Parameters.Add("@imagedata", SqlDbType.Image).Value =dr["Image"];
                    int count = cmd.ExecuteNonQuery();
                    connection.Close();
                }
                BindGridData();
            }
        }
    }
    private void BindGridData()
    {      
        SqlConnection connection = new SqlConnection(DataManager.OraConnString());
        SqlCommand command = new SqlCommand("SELECT imagename,ImageID,Image from [TemporaryImage]", connection);
        SqlDataAdapter daimages = new SqlDataAdapter(command);
        DataTable dt = new DataTable();
        daimages.Fill(dt);
        dgImage.DataSource = dt;
        Session["Img"]=dt;
        dgImage.DataBind();
        dgImage.Attributes.Add("bordercolor", "black");
    }
    protected void btnUpload_Click(object sender, EventArgs e)
    {
        try 
        {
            if (imgUpload.HasFile)
            {
                int width = 145;
                int height = 165;
                byte[] Photo;
                string pp = imgUpload.PostedFile.ContentType;
                if (pp.ToUpper().Contains("JPG") || pp.ToUpper().Contains("PNG") || pp.ToUpper().Contains("JPEG"))
                {
                    using (System.Drawing.Bitmap img = DataManager.ResizeImage(new System.Drawing.Bitmap(imgUpload.PostedFile.InputStream), width, height, DataManager.ResizeOptions.ExactWidthAndHeight))
                    {
                        DataTable dt=null;
                        if (Session["Img"] == null)
                        {
                            dt = new DataTable();
                            dt.Columns.Add("ImageID", typeof(string));
                            dt.Columns.Add("imagename", typeof(string));
                            dt.Columns.Add("Image", typeof(byte[]));
                            //DataRow dr = dt.NewRow();       
                            //dt.Rows.Add(dr); 
                            Session["Img"] = dt;
                        }                      

                        imgUpload.PostedFile.InputStream.Close();
                        Photo = DataManager.ConvertImageToByteArray(img, System.Drawing.Imaging.ImageFormat.Png);
                        string base64String = Convert.ToBase64String(Photo, 0, Photo.Length);
                        dt = (DataTable)Session["Img"];
                        dt.Rows.Add(imgUpload.FileName, dt.Rows.Count + 1, Photo);
                        Session["Img"] = dt;
                        //img.Dispose();
                        //ShiftmentItemsManager.SaveImageTemporary(dt.Rows.Count, Photo);
                        //dgImage.DataSource = dt;
                        //dgImage.DataBind();                  
                        //imgStd.ImageUrl = "data:image/png;base64," + base64String;
                        SqlConnection connection = new SqlConnection(DataManager.OraConnString());
                        int length = imgUpload.PostedFile.ContentLength;
                        byte[] imgbyte = new byte[length];
                        HttpPostedFile img1 = imgUpload.PostedFile;
                        img1.InputStream.Read(imgbyte, 0, length);
                        connection.Open();
                        SqlCommand cmd = new SqlCommand("INSERT INTO TemporaryImage (ImageName,ImageID,Image) VALUES (@ImageName,@ImageID,@imagedata)", connection);
                        cmd.Parameters.Add("@ImageName", SqlDbType.Text, 50).Value = imgUpload.FileName;
                        cmd.Parameters.Add("@ImageID", SqlDbType.Int, 50).Value = dt.Rows.Count;
                        cmd.Parameters.Add("@imagedata", SqlDbType.Image).Value = Photo;
                        int count = cmd.ExecuteNonQuery();
                        connection.Close();
                        if (count == 1)
                        {
                            BindGridData();
                        }
                    }
                }
                else
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Please Select Image Type of (*.PNG) Or (*.JPG) ..!!');", true);
                }
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Please Select File Then Upload!!');", true);
            }

        }
        catch (FormatException fex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('" + fex.Message + "');", true);
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("Database"))
                ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Database Maintain Error. Contact to the Software Provider..!!');", true);
            else
                ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('There is some problem to do the task. Try again properly.!!');", true);
        }
    }
    protected void dgImage_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        if (Session["Img"] != null)
        {
            DataTable dtDtlGrid = (DataTable)Session["Img"];
            dtDtlGrid.Rows.RemoveAt(dgImage.Rows[e.RowIndex].DataItemIndex);
            string ID = dgImage.Rows[dgImage.Rows[e.RowIndex].DataItemIndex].Cells[2].Text;
            IdManager.getInsertUpdateDelete("DELETE FROM [TemporaryImage] Where ImageID='" + ID + "' ");
            BindGridData();
        }
        else
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Your session is over. Try it again!!');", true);
        }
    }
    protected void dgImage_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {
            if (e.Row.RowType == DataControlRowType.DataRow | e.Row.RowType == DataControlRowType.Header | e.Row.RowType == DataControlRowType.Footer)
            {
                e.Row.Cells[2].Attributes.Add("style", "display:none");
            }
        }
        catch (FormatException fex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('" + fex.Message + "');", true);
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("Database"))
                ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Database Maintain Error. Contact to the Software Provider..!!');", true);
            else
                ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('There is some problem to do the task. Try again properly.!!');", true);
        }
    }
    private void getImage()
    {
        DataTable dt = new DataTable();
        dt.Columns.Add("ID", typeof(string));
        dt.Columns.Add("Image", typeof(byte[]));
        //DataRow dr = dt.NewRow();       
        //dt.Rows.Add(dr); 
        Session["Img"] = dt;
    }
}