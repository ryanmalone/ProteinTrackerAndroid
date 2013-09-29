using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using ServiceStack.ServiceClient.Web;
using ProteinTrackerMVC.api;
using System.Collections.Generic;
using System.Linq;

namespace ProteinTrackerAndroid
{
	[Activity (Label = "Protein Tracker", MainLauncher = true)]
	public class MainActivity : Activity
	{
		private JsonServiceClient client;
		private IList<User> users;

		 

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			client = new JsonServiceClient ("http://10.1.1.109/api");


			PopulateSelectUsers ();

			Spinner usersSpinner = FindViewById<Spinner> (Resource.Id.usersSpinner);
			usersSpinner.ItemSelected += (object sender, AdapterView.ItemSelectedEventArgs e) => 
			{
				var selectedUser = users[usersSpinner.SelectedItemPosition];
				TextView goalTextView = FindViewById<TextView> (Resource.Id.GoalTotal);
				TextView totalTextView = FindViewById<TextView> (Resource.Id.Total);

				goalTextView.Text = selectedUser.Goal.ToString();
				totalTextView.Text = selectedUser.Total.ToString();


			};

			// event for add user
		var addUserButton = FindViewById<Button> (Resource.Id.AddUser);
			addUserButton.Click += (object sender, EventArgs e) => {
				TextView goalTextView = FindViewById<TextView> (Resource.Id.Goal);
				TextView nameTextView = FindViewById<TextView> (Resource.Id.Name);
				var goal = int.Parse (goalTextView.Text);
				var response = client.Send (new AddUser { Goal = goal, Name = nameTextView.Text });

				PopulateSelectUsers ();
				goalTextView.Text = string.Empty;
				nameTextView.Text = string.Empty;

				Toast.MakeText (this, "Added new user", ToastLength.Short).Show();


			};

			// event for add Protein
			var addProteinButton = FindViewById<Button> (Resource.Id.AddProtein);
			addProteinButton.Click += (object sender, EventArgs e) => {
				TextView amountTextView = FindViewById<TextView> (Resource.Id.Amount);
				var amount = int.Parse(amountTextView.Text);
				var selectedUser = users[usersSpinner.SelectedItemPosition];

				var response = client.Send (new AddProtein { Amount = amount, UserID = selectedUser.Id });
				selectedUser.Total = response.NewTotal;
				TextView totalTextView = FindViewById<TextView> (Resource.Id.Total);
				totalTextView.Text = selectedUser.Total.ToString();
				amountTextView.Text = string.Empty;

				//string usermessage =  "Added Protein to :" & selectedUser.Name;

				Toast.MakeText (this, "Added Protein to :" + selectedUser.Name, ToastLength.Short).Show();


			};

	

			client.GetAsync<UserResponse>("users",(w2) => {
				Console.WriteLine(w2);
				foreach(var c in w2.Users) {
					Console.WriteLine(c);
					Console.WriteLine(c.Name);

				}
			}, 
			(w2, ex) => {
				Console.WriteLine(ex.Message);
			});
		
		}

		void PopulateSelectUsers ()
		{

		

			var response = client.Get<UserResponse> ("users");

				users = response.Users.ToList ();

				var names = users.Select (u => u.Name);

				var usersSpiner = FindViewById<Spinner> (Resource.Id.usersSpinner);
				usersSpiner.Adapter = new ArrayAdapter<string> (this, Android.Resource.Layout.SimpleListItem1, names.ToArray());
			
						



		}
	}
}


