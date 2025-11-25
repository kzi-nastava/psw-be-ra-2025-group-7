using System;
using System.Diagnostics.Eventing.Reader;
using Explorer.BuildingBlocks.Core.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Core.Domain.Entities
{
	public class Question : Entity
    {
		public string Content { get;  set; }
		public bool AllowsMultipleCorrect { get; set; }

        public long QuizId { get; set; }
        public Quiz Quiz { get; set; }

        public ICollection<Option> Options { get; set; } = null!;


        protected Question() { } // potreban za EF

		public Question(long quizId, string content, bool allowsMultipleCorrect, IEnumerable<Option> options = null) 
		{
			if (quizId <= 0)
				throw new ArgumentException("QuizId must be a positive number.", nameof(quizId));
			
            QuizId = quizId;
            Content = content;
            AllowsMultipleCorrect = allowsMultipleCorrect;
        }

        public void AddOption(Option option)
        {
            if (Options.Count >= 5)
                throw new InvalidOperationException("Question cannot have more than 5 options.");

            // Provera da li je dozvoljeno vise tacnih odgovora
            if (!AllowsMultipleCorrect && option.IsCorrect && Options.Any(o => o.IsCorrect))
                throw new InvalidOperationException("Only one correct option allowed for this question.");

            Options.Add(option);
        }

       
    }
}
