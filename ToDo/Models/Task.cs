using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System;

namespace ToDo.Models
{
  public class Task
  {
    private int _id;
    private string _description;

    public Task(string Description, int Id = 0)
    {
      _id = Id;
      _description = Description;
    }

    public override bool Equals(System.Object otherTask)
    {
      if (!(otherTask is Task))
      {
        return false;
      }
      else
      {
        Task newTask = (Task) otherTask;
        bool idEquality = (this.GetId() == newTask.GetId());
        bool descriptionEquality = (this.GetDescription() == newTask.GetDescription());
        return (idEquality && descriptionEquality);
      }
    }

    public override int GetHashCode()
    {
      return this.GetId().GetHashCode();
    }

    public string GetDescription()
    {
      return _description;
    }

    public int GetId()
    {
      return _id;
    }

    public void Save()
     {
       MySqlConnection conn = DB.Connection();
       conn.Open();

       var cmd = conn.CreateCommand() as MySqlCommand;
       cmd.CommandText = @"INSERT INTO `tasks` (`description`) VALUES (@TaskDescription);";

       MySqlParameter description = new MySqlParameter();
       description.ParameterName = "@TaskDescription";
       description.Value = this._description;
       cmd.Parameters.Add(description);

       cmd.ExecuteNonQuery();
       _id = (int) cmd.LastInsertedId;
       conn.Close();
        if (conn != null)
        {
            conn.Dispose();
        }
     }

    //...GETTERS AND SETTERS HERE...

    public static List<Task> GetAll()
    {
      List<Task> allTasks = new List<Task> {};
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT * FROM tasks;";
      var rdr = cmd.ExecuteReader() as MySqlDataReader;
      while(rdr.Read())
      {
        int taskId = rdr.GetInt32(0);
        string taskName = rdr.GetString(1);
        Task newTask = new Task(taskName, taskId);
        allTasks.Add(newTask);
      }
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
      return allTasks;
    }

    public static void DeleteAll()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"DELETE FROM tasks;";
      cmd.ExecuteNonQuery();
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
    }



    public static Task Find(int id)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT * FROM `tasks` WHERE id = @thisId;";

      MySqlParameter thisId = new MySqlParameter();
      thisId.ParameterName = "@thisId";
      thisId.Value = id;
      cmd.Parameters.Add(thisId);

      var rdr = cmd.ExecuteReader() as MySqlDataReader;

      int taskId = 0;
      string taskDescription = "";

      while (rdr.Read())
      {
        taskId = rdr.GetInt32(0);
        taskDescription = rdr.GetString(1);
      }
      Task foundTask= new Task(taskDescription, taskId);
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
      return foundTask;
    }

    public void AddCategory(Category newCategory)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"INSERT INTO categories_tasks (category_id, task_id) VALUES (@CategoryId, @TaskId);";

      MySqlParameter category_id = new MySqlParameter();
      category_id.ParameterName = "@CategoryId";
      category_id.Value = newCategory.GetId();
      cmd.Parameters.Add(category_id);

      MySqlParameter task_id = new MySqlParameter();
      task_id.ParameterName = "@TaskId";
      task_id.Value = _id;
      cmd.Parameters.Add(task_id);

      cmd.ExecuteNonQuery();
      conn.Close();
      if (conn != null)
      {
          conn.Dispose();
      }
    }


    public List<Category> GetCategories()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT categories.*
        FROM categories
        JOIN categories_tasks ON (categories.id = categories_tasks.category_id)
        JOIN tasks ON(tasks.id = categories_tasks.task_id)
        WHERE task_id = @taskId;";

      MySqlParameter taskIdParameter = new MySqlParameter();
      taskIdParameter.ParameterName = "@taskId";
      taskIdParameter.Value = _id;
      cmd.Parameters.Add(taskIdParameter);

      var rdr = cmd.ExecuteReader() as MySqlDataReader;
      List<Category> categories = new List<Category> {};
        while(rdr.Read())
        {
          int thisCategoryId = rdr.GetInt32(0);
          string categoryName = rdr.GetString(1);
          Category foundCategory = new Category(categoryName, thisCategoryId);
          categories.Add(foundCategory);
        }
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
      return categories;
    }

    public void Delete()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      MySqlCommand cmd = new MySqlCommand("DELETE FROM tasks WHERE id = @TaskId; DELETE FROM categories_tasks WHERE task_id = @TaskId;", conn);
      MySqlParameter taskIdParameter = new MySqlParameter();
      taskIdParameter.ParameterName = "@TaskId";
      taskIdParameter.Value = this.GetId();

      cmd.Parameters.Add(taskIdParameter);
      cmd.ExecuteNonQuery();

      if (conn != null)
      {
        conn.Close();
      }
    }

  }
}
