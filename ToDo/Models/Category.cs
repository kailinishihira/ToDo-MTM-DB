using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System;

namespace ToDo.Models
{
  public class Category
  {
    private string _name;
    private int _id;

    public Category(string name, int id = 0)
    {
      _name = name;
      _id = id;
    }
    public override bool Equals (System.Object otherCategory)
    {
      if (!(otherCategory is Category))
      {
        return false;
      }
      else
      {
        Category newCategory = (Category) otherCategory;
        return this.GetId().Equals(newCategory.GetId());
      }
    }

    public override int GetHashCode()
    {
      return this.GetId().GetHashCode();
    }

    public string GetName()
    {
      return _name;
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
      cmd.CommandText = @"INSERT INTO `categories`  (`name`) VALUES (@name);";
      MySqlParameter name = new MySqlParameter();
      name.ParameterName = "@name";
      name.Value = this._name;
      cmd.Parameters.Add(name);

      cmd.ExecuteNonQuery();
      _id = (int) cmd.LastInsertedId;
    }

    public static void DeleteAll()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"DELETE FROM categories;";
      cmd.ExecuteNonQuery();
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
    }

    public static List<Category> GetAll()
    {
      List<Category> allCategories = new List<Category>{};
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT * FROM categories;";
      var rdr = cmd.ExecuteReader() as MySqlDataReader;

      while(rdr.Read())
      {
        int CategoryId = rdr.GetInt32(0);
        string CategoryName = rdr.GetString(1);
        Category newCategory = new Category(CategoryName, CategoryId);
        allCategories.Add(newCategory);
      }
      return allCategories;
    }

    public static Category Find(int id)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT * FROM categories WHERE id = (@searchId);";

      MySqlParameter searchId = new MySqlParameter();
      searchId.ParameterName = "@searchId";
      searchId.Value = id;
      cmd.Parameters.Add(searchId);

      var rdr = cmd.ExecuteReader() as MySqlDataReader;
      int CategoryId = 0;
      string CategoryName ="";

      while (rdr.Read())
      {
        CategoryId = rdr.GetInt32(0);
        CategoryName = rdr.GetString(1);
      }
      Category newCategory = new Category(CategoryName,CategoryId);
      return newCategory;
    }

    public static void DeleteCategory(int id)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"DELETE FROM categories WHERE id = @thisId;";

      MySqlParameter categoryId = new MySqlParameter();
      categoryId.ParameterName = "@thisId";
      categoryId.Value = id;
      cmd.Parameters.Add(categoryId);

      cmd.ExecuteNonQuery();
    }

    public void Delete()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      MySqlCommand cmd = new MySqlCommand("DELETE FROM categories WHERE id = @CategoryId; DELETE FROM categories_tasks WHERE category_id = @CategoryId;", conn);
      MySqlParameter categoryIdParameter = new MySqlParameter();
      categoryIdParameter.ParameterName = "@CategoryId";
      categoryIdParameter.Value = this.GetId();

      cmd.Parameters.Add(categoryIdParameter);
      cmd.ExecuteNonQuery();

      if (conn != null)
      {
        conn.Close();
      }
    }

   public List<Task> GetTasks()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT tasks.*
          FROM categories
        JOIN categories_tasks ON (categories.id = categories_tasks.category_id)
        JOIN tasks ON (categories_tasks.task_id = tasks.id)
        WHERE categories.id = @CategoryId;";

      MySqlParameter categoryIdParameter = new MySqlParameter();
      categoryIdParameter.ParameterName = "@CategoryId";
      categoryIdParameter.Value = _id;
      cmd.Parameters.Add(categoryIdParameter);

      var rdr = cmd.ExecuteReader() as MySqlDataReader;
      List<Task> tasks = new List<Task> {};
      while(rdr.Read())
        {
          int thisTaskId = rdr.GetInt32(0);
          string taskDescription = rdr.GetString(1);
          Task foundTask = new Task(taskDescription, thisTaskId);
          tasks.Add(foundTask);
        }
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
      return tasks;
    }

    public void AddTask(Task newTask)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"INSERT INTO categories_tasks (category_id, task_id) VALUES (@CategoryId, @TaskId);";

      MySqlParameter category_id = new MySqlParameter();
      category_id.ParameterName = "@CategoryId";
      category_id.Value = _id;
      cmd.Parameters.Add(category_id);

      MySqlParameter task_id = new MySqlParameter();
      task_id.ParameterName = "@TaskId";
      task_id.Value = newTask.GetId();
      cmd.Parameters.Add(task_id);

      cmd.ExecuteNonQuery();
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
    }


  }
}
