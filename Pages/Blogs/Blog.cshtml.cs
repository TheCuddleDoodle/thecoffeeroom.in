using Coffeeroom.Core.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Text.Json;
using System.Web;
using Serilog;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Coffeeroom.Pages.Blogs
{
    public class Comments
    {
        public int Id { get; set; }
        public string Comment { get; set; }
        public string DateCommented { get; set; }
        public int UserId { get; set; }
        public string UsersName { get; set; }
        public string UserName { get; set; }
    }

    public class BlogComment
    {
        public string BlogUrl { get; set; }
        public string Comment { get; set; }
        public int CommentId { get; set; }
        public string CommentText { get; set; }
    }

    public class BlogReply
    {
        public string BlogUrl { get; set; }
        public string CommentId { get; set; }
        public string Reply { get; set; }
        public int ReplyId { get; set; }
        public string ReplyText { get; set; }
    }
    [IgnoreAntiforgeryToken]
    public class BlogModel : PageModel
    {
        public int BlogId { get; set; }
        public string BlogTags { get; set; }
        public string BlogTitle { get; set; }
        public string UrlHandle { get; set; }
        public string YearPosted { get; set; }
        public List<Comments> comments { get; set; }
        public int commentscounter { get; set; }
        readonly string connectionString = ConfigHelper.NewConnectionString;
        string message = "something went wrong", type = "error";

        public void OnGet(string url, string year)
        {
            if (url != null && year != null)
            {
                UrlHandle = url;
                YearPosted = year;
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var command = new SqlCommand("SELECT Id, Tags,Title,UrlHandle FROM TblBlogMaster WHERE UrlHandle = @urlhandle", connection);
                command.Parameters.AddWithValue("@urlhandle", UrlHandle);
                var reader = command.ExecuteReader();
                string tags = string.Empty;
                if (reader.Read())
                {
                    BlogId = (int)reader["Id"];
                    BlogTags = reader["Tags"].ToString();
                    BlogTitle = reader["Title"].ToString();
                    UrlHandle = reader["UrlHandle"].ToString();
                }
                reader.Close();
            }
            else
            {
                Response.Redirect("/blogs");
            }
        }
        public async Task<IActionResult> OnPostLdCommentsAsync(string url)
        {
            Dictionary<int, dynamic> comments = new Dictionary<int, dynamic>();
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            var command = new SqlCommand(@"SELECT
						  c.Id,
						  c.Comment,
						  c.UserId,
						  u.FirstName,
						  u.LastName,
						  u.UserName as CommentUserName,
						  u.Id,
						  LEFT(c.DatePosted, 12),
						  a.Image,
						  r.Id AS ReplyId,
						  r.Reply AS ReplyComment,						  
						LEFT(r.DatePosted, 12),
						  u2.Id AS ReplyUserId,
						  u2.FirstName AS ReplyFirstName,
						  u2.LastName AS ReplyLastName,
						  a2.Image AS ReplyImage,
						u2.UserName AS ReplyUserName
						FROM
						  TblBlogComment c
						  JOIN TblUserProfile u ON c.UserId = u.Id
						  JOIN TblAvatarMaster a ON u.AvatarId = a.Id
						  LEFT JOIN TblBlogReply r ON c.Id = r.CommentId
                          LEFT JOIN TblBlogMaster bm ON bm.Id = c.PostId
						  LEFT JOIN TblUserProfile u2 ON r.UserId = u2.Id
						  LEFT JOIN TblAvatarMaster a2 ON u2.AvatarId = a2.Id
            where bm.UrlHandle = @posturl
						ORDER BY
						  c.DatePosted;
						", connection);
            command.Parameters.AddWithValue("@posturl", url);
            var reader = await command.ExecuteReaderAsync();
            string user = "";
            bool editable = false, replyeditable = false;
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    if (HttpContext.Session.GetString("username") != null)
                    {
                        user = "yes";
                        if (reader.GetString(5) == HttpContext.Session.GetString("username").ToString())
                        {
                            editable = true;
                        }
                        else
                        {
                            editable = false;
                        }
                        try
                        {
                            if (reader.GetString(16) == HttpContext.Session.GetString("username").ToString())
                            {
                                replyeditable = true;
                            }
                            else
                            {
                                replyeditable = false;
                            }
                        }
                        catch
                        {
                            replyeditable = false;
                        }
                    }
                    else
                    {
                        user = "no";
                    }
                    var commentId = reader.GetInt32(0);
                    if (!comments.ContainsKey(commentId))
                    {
                        var comment = new
                        {
                            id = commentId,
                            edit = editable,
                            user = user,
                            fullname = reader.GetString(3) + " " + reader.GetString(4),
                            userid = reader.GetInt32(2),
                            username = reader.GetString(5),
                            comment = HttpUtility.HtmlDecode(reader.GetString(1)),
                            date = reader.GetString(7),
                            avatar = reader.GetString(8),
                            replies = new List<object>()
                        };

                        comments.Add(commentId, comment);
                    }

                    if (!reader.IsDBNull(9))
                    {
                        var reply = new
                        {
                            replyEdit = replyeditable,
                            user = user,
                            replyId = reader.GetInt32(9),
                            replyComment = HttpUtility.HtmlDecode(reader.GetString(10)),
                            replyUserId = reader.GetInt32(12),
                            replyDate = reader.GetString(11),
                            //replyFirstName = reader.GetString(13),
                            //replyLastName = reader.GetString(14),
                            replyFullName = reader.GetString(13) + " " + reader.GetString(14),
                            replyAvatar = reader.GetString(15)
                        };

                        comments[commentId].replies.Add(reply);
                    }
                }
            }
            else
            {
                //return StatusCode(404);
                string errorMessage = "error";
                return BadRequest(new { error = errorMessage }); // HTTP 400 Bad Request
            }
           
            await reader.CloseAsync();
            await connection.CloseAsync();
          //  string json = JsonSerializer.Serialize(comments.Values);
            return new JsonResult(comments.Values);
        }

        public async Task<JsonResult> OnPostAddCommentAsync([FromBody] BlogComment blogComment)
        {
           
            string userid = "", blogid = "";


            if (blogComment.Comment != null && blogComment.BlogUrl != null)
            {

                try
                {
                    string encodedcomment = HttpUtility.HtmlEncode(blogComment.Comment.ToString().Trim());
                    if (encodedcomment.Length >= 3)
                    {
                        using var connection = new SqlConnection(connectionString);
                        await connection.OpenAsync();
                        var command = new SqlCommand("SELECT Id FROM TblBlogMaster WHERE UrlHandle = @urlhandle", connection);
                        command.Parameters.AddWithValue("@urlhandle", blogComment.BlogUrl);
                        var reader = await command.ExecuteReaderAsync();

                        if (await reader.ReadAsync())
                        {
                            blogid = reader.GetInt32(0).ToString();
                        }
                        reader.Close();
                        command = new SqlCommand("SELECT Id FROM TblUserProfile WHERE UserName = @username", connection);
                        command.Parameters.AddWithValue("@username", HttpContext.Session.GetString("username").ToString());
                        reader = await command.ExecuteReaderAsync();

                        if (await reader.ReadAsync())
                        {
                            userid = reader.GetInt32(0).ToString();
                        }
                        reader.Close();
                        SqlCommand maxIdCommand = new("SELECT ISNULL(MAX(Id), 0) + 1 FROM TblBlogComment", connection);
                        int newId = Convert.ToInt32(maxIdCommand.ExecuteScalar());

                        command = new SqlCommand("insert into TblBlogComment(Id,Comment,UserId,PostId,IsActive,DatePosted) values(" + newId + ",'" + encodedcomment + "'," + userid + "," + blogid + ",1,@dateposted)", connection);
                        command.Parameters.Add("@dateposted", SqlDbType.DateTime).Value = DateTime.Now;
                        await command.ExecuteNonQueryAsync();
                        message = "submitted ";
                        type = "success";
                    }
                    else
                    {
                        message = "comment too short ";
                        type = "error";
                    }

                }
                catch (Exception ex)
                {
                    Log.Information("error in adding comment by" + HttpContext.Session.GetString("username") + "on a blog :" + ex.Message.ToString());
                    message = "something went wrong";
                }
            }
            else
            {
                message = "something went wrong:";
            }
            var keys = new { message, type };
            return new JsonResult(keys);
        }

        public async Task<JsonResult> OnPostAddReplyAsync([FromBody] BlogReply blogReply)
        {
            string userid = "";
            string encodedreply = HttpUtility.HtmlEncode(blogReply.ReplyText.ToString().Trim());
            if (blogReply.BlogUrl != null)
            {
                try
                {
                    if (encodedreply.Trim() != "")
                    {
                        using var connection = new SqlConnection(connectionString);
                        await connection.OpenAsync();
                        var command = new SqlCommand("SELECT Id FROM TblUserProfile WHERE UserName = @username", connection);
                        command.Parameters.AddWithValue("@username", HttpContext.Session.GetString("username").ToString());
                        var reader = await command.ExecuteReaderAsync();
                        if (await reader.ReadAsync())
                        {
                            userid = reader.GetInt32(0).ToString();
                        }
                        reader.Close();
                        SqlCommand maxIdCommand = new("SELECT ISNULL(MAX(Id), 0) + 1 FROM TblBlogReply", connection);
                        int newId = Convert.ToInt32(maxIdCommand.ExecuteScalar());

                        command = new SqlCommand("insert into TblBlogReply(Id,CommentId,UserId,Reply,IsActive,DatePosted) values(" + newId + ",'" + blogReply.CommentId + "','" + userid + "','" + encodedreply + "',1,@dateposted)", connection);
                        command.Parameters.Add("@dateposted", SqlDbType.DateTime).Value = DateTime.Now;
                        await command.ExecuteNonQueryAsync();
                        message = "submitted ";
                        type = "success";
                    }
                    else
                    {
                        message = "Reply too short";
                        type = "error";
                    }

                }
                catch (Exception ex)
                {
                    Log.Information("error in adding reply by" + HttpContext.Session.GetString("username") + "on a blog :" + ex.Message.ToString());
                    message = "something went wrong";
                }
            }
            else
            {
                message = "something went wrong:";
            }
            var keys = new { message, type };
            return new JsonResult(keys);
        }

        public async Task<JsonResult> OnPostDelAsync(int id, string type)
        {

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            if (type == "comment")
            {

                using var transaction = connection.BeginTransaction();
                try
                {
                    // Perform multiple database operations within the transaction
                    using var command1 = connection.CreateCommand();
                    command1.Transaction = transaction;
                    command1.CommandText = "DELETE FROM TblBlogComment WHERE Id = @id and UserId = @user_id";
                    command1.Parameters.AddWithValue("@id", id);
                    command1.Parameters.AddWithValue("@user_id", HttpContext.Session.GetString("user_id"));

                    await command1.ExecuteNonQueryAsync();

                    using var command2 = connection.CreateCommand();
                    command2.Transaction = transaction;
                    command2.CommandText = "DELETE FROM TblBlogReply WHERE CommentId = @id";
                    command2.Parameters.AddWithValue("@id", id);
                    command2.Parameters.AddWithValue("@user_id", HttpContext.Session.GetString("user_id"));
                    await command2.ExecuteNonQueryAsync();
                    transaction.Commit();
                    message = "comment deleted!";
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
            else if (type == "reply")
            {
                var command = new SqlCommand("DELETE FROM TblBlogReply WHERE Id = @id and UserId = @user_id", connection);
                command.Parameters.AddWithValue("@id", id);
                command.Parameters.AddWithValue("@user_id", HttpContext.Session.GetString("user_id"));
                await command.ExecuteNonQueryAsync();
                message = "reply deleted!";

            }
            await connection.CloseAsync();

            var keys = new { message, type };
            return new JsonResult(keys);
        }

        //public async Task<JsonResult> OnDelete(int id, string typ)
        //{

        //    using var connection = new SqlConnection(connectionString);
        //    await connection.OpenAsync();
        //    if (typ == "comment")
        //    {

        //        using var transaction = connection.BeginTransaction();
        //        try
        //        {
        //            // Perform multiple database operations within the transaction
        //            using var command1 = connection.CreateCommand();
        //            command1.Transaction = transaction;
        //            command1.CommandText = "DELETE FROM TblBlogComment WHERE Id = @id and UserId = @user_id";
        //            command1.Parameters.AddWithValue("@id", id);
        //            command1.Parameters.AddWithValue("@user_id", HttpContext.Session.GetString("user_id"));

        //            await command1.ExecuteNonQueryAsync();

        //            using var command2 = connection.CreateCommand();
        //            command2.Transaction = transaction;
        //            command2.CommandText = "DELETE FROM TblBlogReply WHERE CommentId = @id";
        //            command2.Parameters.AddWithValue("@id", id);
        //            command2.Parameters.AddWithValue("@user_id", HttpContext.Session.GetString("user_id"));
        //            await command2.ExecuteNonQueryAsync();
        //            transaction.Commit();
        //            message = "deleted";
        //            type = "success";
        //        }
        //        catch (Exception)
        //        {
        //            transaction.Rollback();
        //            throw;
        //        }
        //    }
        //    else if (typ == "reply")
        //    {
        //        var command = new SqlCommand("DELETE FROM TblBlogReply WHERE Id = @id and UserId = @user_id", connection);
        //        command.Parameters.AddWithValue("@id", id);
        //        command.Parameters.AddWithValue("@user_id", HttpContext.Session.GetString("user_id"));
        //        await command.ExecuteNonQueryAsync();

        //    }
        //    await connection.CloseAsync();

        //    var keys = new { message, type };
        //    return new JsonResult(keys);
        //}
        //edit comments
        public async Task<JsonResult> OnPostCommentsEditAsync(int id, string comment)
        {
            try
            {
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();
                var sql = "UPDATE TblBlogComment SET Comment = @Commentval WHERE Id = @Idval AND UserId = @UserId";
                var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Commentval", HttpUtility.HtmlEncode(comment));
                command.Parameters.AddWithValue("@Idval", id);
                command.Parameters.AddWithValue("@UserId", HttpContext.Session.GetString("user_id").ToString());
                await command.ExecuteNonQueryAsync();
                await connection.CloseAsync();
                message = "comment saved";
                type = "success";
            }
            catch (Exception ex)
            {
                message = "somethng went wrong" + ex.Message.ToString();
                type = "error";
            }
            var keys = new { message, type };
            return new JsonResult(keys);
        }

        //edit replies
        public async Task<JsonResult> OnPostReplyEditAsync(int id, string reply)
        {
            try
            {
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();
                var sql = "UPDATE TblBlogReply SET Reply = @Replyval WHERE Id = @Idval AND UserId = @UserId ";
                var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Replyval", HttpUtility.HtmlEncode(reply));
                command.Parameters.AddWithValue("@Idval", id);
                command.Parameters.AddWithValue("@UserId", HttpContext.Session.GetString("user_id").ToString());

                await command.ExecuteNonQueryAsync();

                await connection.CloseAsync();

                message = "reply saved";
                type = "success";
            }
            catch (Exception ex)
            {
                message = "somethng went wrong" + ex.Message.ToString();
                type = "error";
            }
            var keys = new { message, type };
            return new JsonResult(keys);
        }

        //load authors
        public JsonResult OnGetLoadAuthors(int blogId)
        {
            List<object> data = new();
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var command = new SqlCommand("SELECT ba.AuthorId,u.UserName,u.Bio,u.FirstName,u.LastName,avt.Image FROM TblBlogMaster AS b " +
                                        "JOIN TblBlogAuthor AS ba ON ba.BlogId = b.Id " +
                                        "JOIN TblUserProfile AS u ON ba.AuthorId = u.Id " +
                                        "JOIN TblAvatarMaster AS avt ON u.AvatarId = avt.Id where b.Id = @BlogId ", connection);
            //added scalar vars
            command.Parameters.AddWithValue("@BlogId", blogId);
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var row = new
                {
                    userId = reader.GetInt32(0),
                    userName = reader.GetString(1),
                    userBio = reader.GetString(2),
                    firstName = reader.GetString(3),
                    lastName = reader.GetString(4),
                    avatarImage = reader.GetString(5),
                };

                data.Add(row);
            }
            reader.Close();
            connection.Close();
            string json = JsonSerializer.Serialize(data);
            return new JsonResult(json);
        }
    }
}
