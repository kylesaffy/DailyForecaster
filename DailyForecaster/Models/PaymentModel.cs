using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DailyForecaster.Models
{
	public class PaymentModel
	{
		public string PaymentModelId { get; set; }
		public string AccountAmortisationId { get; set; }
		[ForeignKey("AccountAmortisationId")]
		public AccountAmortisation AccountAmortisation { get; set; }
		public double LoanInstallment { get; set; }
		public double NonLoanPortion { get; set; }
		public double CostOfLoan { get; set; }
		public double AdditionalLoan { get; set; }
		public double TotalPayable { get; set; }
		public PaymentModel() { }
		public PaymentModel (Account account,string amortId)
		{	  		
			PaymentModelId = Guid.NewGuid().ToString();
			AccountAmortisationId = amortId;
			NonLoanPortion = account.MonthlyFee;
			AdditionalLoan = 0;
			LoanInstallment = account.MonthlyPayment;
			CostOfLoan = account.MonthlyPayment - account.MonthlyFee;
			TotalPayable = CostOfLoan;
		}
		
	}
}
