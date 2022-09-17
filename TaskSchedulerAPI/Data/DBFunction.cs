using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using TaskSchedulerAPI.Models;

namespace TaskSchedulerAPI.Data {
    public static class DBFunction {
        static private string connectionString = "connectionstring";

        #region *** User methods ***
        /// <summary>
        /// Перевірка наявності користувача в БД
        /// </summary>
        /// <param name="model">Об'єкт LoginModel, що містить пароль та логін користувача</param>
        /// <returns>true якщо запис про користувача є в БД, в іншому випадку false</returns>
        static public bool AuthorizationUser(LoginModel model) {
            using (SqlConnection connection = new SqlConnection(connectionString)) {
                connection.Open();
                string sqlExpression = $"SELECT COUNT(*) FROM users WHERE UserName = @userName AND UserPassword = @userPassword";
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                command.Parameters.Add(new SqlParameter("@userName", model.UserName));
                command.Parameters.Add(new SqlParameter("@userPassword", model.UserPassword));
                if ((int)command.ExecuteScalar() != 0) return true;
                return false;
            }
        }

        /// <summary>
        /// Перевіряє чи в БД є користувачі з певними логіном та/або поштою
        /// </summary>
        /// <param name="userName">Логін користувача</param>
        /// <param name="userEmail">Електронна пошта користувача</param>
        /// <param name="userId">id користувача, запис з таким id не буде перевірятися</param>
        /// <returns>true якщо записи з необхідними параметрами знайдені в БД, в іншому випадку false</returns>
        static public bool Availability_Email_Name(string userName, string userEmail, int userId) {
            using (SqlConnection connection = new SqlConnection(connectionString)) {
                connection.Open();
                string sqlExpression = $"SELECT COUNT(*) FROM Users WHERE (UserName = @userName OR UserEmail = @userEmail) AND NOT UserId = @userId";
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                command.Parameters.Add(new SqlParameter("@userId", userId));
                command.Parameters.Add(new SqlParameter("@userName", userName));
                command.Parameters.Add(new SqlParameter("@userEmail", userEmail));
                if ((int)command.ExecuteScalar() != 0) return true;
                return false;
            }
        }

        /// <summary>
        /// Повертає id користувача за паролем та логіном
        /// </summary>
        /// <param name="model">Об'єкт, що містить пароль та логін користувача</param>
        /// <returns>id користувача</returns>
        static public int GetUserId(LoginModel model) {
            using (SqlConnection connection = new SqlConnection(connectionString)) {
                connection.Open();
                string sqlExpression = $"SELECT UserId FROM Users WHERE UserName = @userName AND UserPassword = @userPassword";
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                command.Parameters.Add(new SqlParameter("@userName", model.UserName));
                command.Parameters.Add(new SqlParameter("@userPassword", model.UserPassword));
                SqlDataReader reader = command.ExecuteReader();
                int id = 0;
                if (reader.Read()) {
                    id = reader.GetInt32(0);
                }
                return id;
            }
        }

        /// <summary>
        /// Додає в БД користувача
        /// </summary>
        /// <param name="model">Об'єкт, що містить необхідну інформацію про користувача</param>
        /// <returns>true у випадку успішного додання даних в БД про нового користувача, в іншому випадку false</returns>
        static public int CreateUser(RegisterModel model) {
            using (SqlConnection connection = new SqlConnection(connectionString)) {
                connection.Open();
                string sqlExpression = $"INSERT INTO Users (UserName, UserEmail, UserPassword) VALUES (@userName, @userEmail, @userPassword)";
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                command.Parameters.Add(new SqlParameter("@userName", model.UserName));
                command.Parameters.Add(new SqlParameter("@userEmail", model.UserEmail));
                command.Parameters.Add(new SqlParameter("@userPassword", model.UserPassword));
                return command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Повертає користувача за його id
        /// </summary>
        /// <param name="userId">id користувача</param>
        /// <returns>Користувач - User</returns>
        static public User GetUserByUserId(int userId) {
            using (SqlConnection connection = new SqlConnection(connectionString)) {
                connection.Open();
                string sqlExpression = $"SELECT * FROM Users WHERE UserId = @userId";
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                command.Parameters.Add(new SqlParameter("@userId", userId));
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows) {
                    User user = null;
                    if (reader.Read()) {
                        user = new User() {
                            UserId = reader.GetInt32(0),
                            UserName = reader.GetString(1),
                            UserEmail = reader.GetString(2),
                            UserPassword = reader.GetString(3),
                        };
                    }
                    return user;
                }
            }
            return null;
        }

        /// <summary>
        /// Оновлює дані про певного користувача
        /// </summary>
        /// <param name="user">Об'єкт користувача, що потрібно оновити</param>
        /// <returns>Кількість змінених записів в БД</returns>
        static public int UpdateUser(User user) {
            using (SqlConnection connection = new SqlConnection(connectionString)) {
                connection.Open();
                string sqlExpression = "UPDATE Users SET UserName = @userName, " +
                    "UserEmail = @userEmail, UserPassword = @userPassword " +
                    "WHERE UserId = @userId";
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                command.Parameters.Add(new SqlParameter("@userId", user.UserId));
                command.Parameters.Add(new SqlParameter("@userName", user.UserName));
                command.Parameters.Add(new SqlParameter("@userEmail", user.UserEmail));
                command.Parameters.Add(new SqlParameter("@userPassword", user.UserPassword));
                return command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Видаляє користувача з БД
        /// </summary>
        /// <param name="userId">id користувача</param>
        /// <returns>Кількість видалених записів з БД</returns>
        static public int DeleteUser(int userId) {
            using (SqlConnection connection = new SqlConnection(connectionString)) {
                connection.Open();
                string sqlExpression = "DELETE FROM Users WHERE UserId = @userId";
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                command.Parameters.Add(new SqlParameter("@userId", userId));
                return command.ExecuteNonQuery();
            }
        }
        #endregion

        #region *** Assignment methods ***
        /// <summary>
        /// Повертає всі завдання користувача, за його id
        /// </summary>
        /// <param name="userId">id користувача</param>
        /// <returns>Список завдань певного користувача</returns>
        static public List<Assignment> GetAssignments(int userId) {
            using (SqlConnection connection = new SqlConnection(connectionString)) {
                connection.Open();
                string sqlExpression = $"SELECT * FROM Assignments WHERE UserId = @userId";
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                command.Parameters.Add(new SqlParameter("@userId", userId));
                SqlDataReader reader = command.ExecuteReader();
                List<Assignment> assignments = new List<Assignment>();
                if (reader.HasRows) {
                    Assignment assignment;
                    while (reader.Read()) {
                        assignment = new Assignment() {
                            AssignmentId = reader.GetInt32(0),
                            AssignmentName = reader.GetString(1),
                            AssignmentDescription = reader.GetString(2),
                            AssignmentTime = reader.GetDateTime(3),
                            UserId = reader.GetInt32(5)
                        };
                        try {
                            assignment.AssignmentState = reader.GetBoolean(4);
                        }
                        catch {
                            assignment.AssignmentState = null;
                        }
                        assignments.Add(assignment);
                    }
                }
                return assignments;
            }
        }

        /// <summary>
        /// Додає завдання певного користувача в БД
        /// </summary>
        /// <param name="assignment">Об'єкт завдання, яке потрібно додати в БД</param>
        /// <returns>id завдання</returns>
        static public int AddAssignment(Assignment assignment) {
            using (SqlConnection connection = new SqlConnection(connectionString)) {
                connection.Open();
                string sqlExpression = "INSERT INTO Assignments (AssignmentName, " +
                    "AssignmentDescription, AssignmentTime, AssignmentState, " +
                    "UserId) VALUES (@assignmentName, @assignmentDescription, " +
                    "@assignmentTime, @assignmentState, @userId); SET @assignmentId=SCOPE_IDENTITY()";
                SqlCommand command = new SqlCommand(sqlExpression, connection);

                command.Parameters.Add(new SqlParameter("@assignmentName", assignment.AssignmentName));
                command.Parameters.Add(new SqlParameter("@assignmentDescription", assignment.AssignmentDescription));
                command.Parameters.Add(new SqlParameter("@assignmentTime", assignment.AssignmentTime));
                command.Parameters.Add(new SqlParameter("@assignmentState", assignment.AssignmentState != null ? assignment.AssignmentState : DBNull.Value));
                command.Parameters.Add(new SqlParameter("@userId", assignment.UserId));
                SqlParameter assignmentId = new SqlParameter {
                    ParameterName = "@assignmentId",
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(assignmentId);

                command.ExecuteNonQuery();
                return (int)assignmentId.Value;
            }
        }

        /// <summary>
        /// Оновлює завдання певного користувача
        /// </summary>
        /// <param name="assignment">Об'єкт завдання, що потрібно оновити</param>
        /// <returns>Кількість змінених записів в БД</returns>
        static public int UpdateAssignment(Assignment assignment) {
            using (SqlConnection connection = new SqlConnection(connectionString)) {
                connection.Open();
                string sqlExpression = "UPDATE Assignments SET AssignmentName = @assignmentName, " +
                    "AssignmentDescription = @assignmentDescription, AssignmentTime = @assignmentTime, " +
                    "AssignmentState = @assignmentState WHERE AssignmentId = @assignmentId";
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                command.Parameters.Add(new SqlParameter("@assignmentName", assignment.AssignmentName));
                command.Parameters.Add(new SqlParameter("@assignmentDescription", assignment.AssignmentDescription));
                command.Parameters.Add(new SqlParameter("@assignmentTime", assignment.AssignmentTime));
                command.Parameters.Add(new SqlParameter("@assignmentState", assignment.AssignmentState != null ? assignment.AssignmentState : DBNull.Value));
                command.Parameters.Add(new SqlParameter("@assignmentId", assignment.AssignmentId));

                return command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Видаляє завдання користувача з БД
        /// </summary>
        /// <param name="assignmentId">id завдання</param>
        /// <param name="userId">id користувача</param>
        /// <returns>Кількість видалених записів з БД</returns>
        static public int DeleteAssignment(int assignmentId, int userId) {
            using (SqlConnection connection = new SqlConnection(connectionString)) {
                connection.Open();
                string sqlExpression = "DELETE FROM Assignments WHERE AssignmentId = @assignmentId AND UserId = @userId";
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                command.Parameters.Add(new SqlParameter("@assignmentId", assignmentId));
                command.Parameters.Add(new SqlParameter("@userId", userId));
                return command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Видаляє всі завдання користувача з БД
        /// </summary>
        /// <param name="userId">id користувача</param>
        static public void DeleteAllAssignment(int userId) {
            using (SqlConnection connection = new SqlConnection(connectionString)) {
                connection.Open();
                string sqlExpression = "DELETE FROM Assignments WHERE UserId = @userId";
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                command.Parameters.Add(new SqlParameter("@userId", userId));
                command.ExecuteNonQuery();
            }
        }
        #endregion
    }
}
