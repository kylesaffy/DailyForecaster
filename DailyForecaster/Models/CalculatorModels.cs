using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyForecaster.Models
{
	public class CalculatorModels
	{
		public double PresentValue { get; set; }
		public double FutureValue { get; set; }
		public double InterestRate { get; set; }
		public int Term { get; set; }
		public double Payment { get; set; }
		public string Type { get; set; }
		public CalculatorModels() { }
		public CalculatorModels Get()
		{
			RateInformation rate = new RateInformation();
			return new CalculatorModels() { InterestRate = rate.GetPrime() };
		}
		public CalculatorModels Calculate()
		{
			this.InterestRate = this.InterestRate / 100;
			switch(this.Type)
			{
				case "payment":
					this.PaymentModel();
					return (this);
				case "affordability":
					this.Affordability();
					return (this);
				case "fvmodelwithpayment":
					this.FVModelWithPayment();
					return (this);
				case "fvmodel":
					this.FVModel();
					return (this);
				default:
					return this;
			}
		}
		public void PaymentModel()
		{
			this.Payment = Math.Round((this.InterestRate + (this.InterestRate / (Math.Pow(1 + this.InterestRate, this.Term) - 1))) * this.PresentValue,0);
			this.InterestRate = Math.Round(this.InterestRate * 100, 3);
		}
		public void Affordability()
		{
			this.PresentValue = Math.Round(this.Payment / Math.Pow(1 + this.InterestRate, this.Term),0);
			this.InterestRate = Math.Round(this.InterestRate * 100, 3);
		}
		public void FVModelWithPayment()
		{
			this.FutureValue = Math.Round(this.Payment * ((Math.Pow(1 + this.InterestRate, this.Term) - 1) / this.InterestRate),0);
			this.InterestRate = Math.Round(this.InterestRate * 100, 3);
		}
		public void FVModel()
		{
			this.FutureValue = Math.Round(this.PresentValue * Math.Pow(1 + this.InterestRate, this.Term),0);
			this.InterestRate = Math.Round(this.InterestRate * 100, 3);
		}
	}
}
