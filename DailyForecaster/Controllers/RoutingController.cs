using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using DailyForecaster.Models;
using Firebase.Auth;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FirebaseAuth = FirebaseAdmin.Auth.FirebaseAuth;

namespace DailyForecaster.Controllers
{
	[Route("[controller]")]
	[EnableCors("AllowOrigin")]
	[ApiController]
	public class RoutingController : ControllerBase
	{
		[Route("Get")]
		[HttpGet]
		public async Task<ActionResult> Get()
		{
			string authHeader = this.HttpContext.Request.Headers["Authorization"];
			FirebaseToken auth = await validate(authHeader);
			string route = this.HttpContext.Request.Headers["Route"];
			if (auth.ExpirationTimeSeconds > DateTimeOffset.Now.ToUnixTimeSeconds())
			{
				switch (route)
				{
					case "UnseenCount":
						return UnseenCount(auth.Claims["email"].ToString());
					case "Index":
						return Index(auth.Claims["email"].ToString());
				}
			}
			return Ok();
		}
		[Route("Post")]
		[HttpPost]
		public async Task<ActionResult> Post([FromBody] JsonElement json)
		{
			string authHeader = this.HttpContext.Request.Headers["Authorization"];
			string route = this.HttpContext.Request.Headers["Route"];
			FirebaseToken auth = await validate(authHeader);
			if (auth.ExpirationTimeSeconds > DateTimeOffset.Now.ToUnixTimeSeconds())
			{
				switch (route)
				{
					case "NewUser":
						new FirebaseUser(auth.Claims["email"].ToString(), auth.Uid);
						return Ok();
				}
			}
			return Ok();
		}
		private async Task<FirebaseToken> validate(string token)
		{
			return await FirebaseAuth.DefaultInstance
				.VerifyIdTokenAsync(token);
		}
		private ActionResult Index(string email)
		{
			IndexModel model = new IndexModel();
			return Ok(model.GetModel(email));
		}
		private ActionResult UnseenCount(string email)
		{
			int count = 0;
			AspNetUsers users = new AspNetUsers();
			string id = users.getUserId(email);
			using (FinPlannerContext _context = new FinPlannerContext())
			{
				List<string> collections = _context
					.UserCollectionMapping
					.Where(x => x.Id == id)
					.Select(x => x.CollectionsId)
					.ToList();
				List<string> accounts = _context
					.Account
					.Where(Acc => collections.Contains(Acc.CollectionsId))
					.Select(x => x.Id)
					.ToList();
				foreach (string item in accounts)
				{
					count = count + _context
						.AutomatedCashFlows
						.Where(auto => item.Contains(auto.AccountId))
						.Where(x => x.Validated == false)
						.Count();
				}
				return Ok(count);
			}
		}
	}
}
