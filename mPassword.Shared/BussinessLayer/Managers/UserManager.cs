using System.Collections.Generic;

namespace mPassword.Shared
{
	public static class UserManager
	{
		public static User GetUser(int id)
		{
			return UserRepository.GetUser(id);
		}

		public static IList<User> GetUsers()
		{
			return new List<User>(UserRepository.GetUsers());
		}

		public static int SaveUser(User user)
		{
			return UserRepository.SaveUser(user);
		}

		public static int DeleteTask(int id)
		{
			return UserRepository.DeleteUser(id);
		}
	}
}