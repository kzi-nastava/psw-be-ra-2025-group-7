using System;
using System.Diagnostics.Eventing.Reader;
using Explorer.BuildingBlocks.Core.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Core.Domain.Entities
{

	public class Option : Entity
	{
		public string Text { get; private set; }
		public bool IsCorrect { get; private set; }
		public string Feedback { get; private set; } // objasnjenje tacnog / netacnog odgovora

		public long QuestionId { get; private set; }
		public Question Question { get; private set; }

		protected Option() { } // potreban za EF

		public Option(long questionId, string text, bool isCorrect, string feedback)
		{
			
            QuestionId = questionId;
            Text = text;
            IsCorrect = isCorrect;
            Feedback = feedback;
        }

	}
}
