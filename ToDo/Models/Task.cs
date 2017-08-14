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
            return allTasks;
        }

        public static void DeleteAll()
        {
          MySqlConnection conn = DB.Connection();
          conn.Open();
          var cmd = conn.CreateCommand() as MySqlCommand;
          cmd.CommandText = @"DELETE FROM tasks;";
          cmd.ExecuteNonQuery();
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
         return foundTask;
      }
  }
}
