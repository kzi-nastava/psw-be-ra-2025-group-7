using System;
using System.Diagnostics.Eventing.Reader;
using Explorer.BuildingBlocks.Core.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Metadata;
using Explorer.Stakeholders.Core.Domain;

namespace Explorer.Tours.Core.Domain.Entities
{
	public class Quiz : Entity
    {
		public long AuthorId { get; set; }
        public User Author { get; set; }

		public string Title { get; set; }

        public ICollection<Question> Questions { get; set; } = null!;

        public Quiz() { }

		private void SetTitle(string title)
		{
			if (string.IsNullOrWhiteSpace(title))
				throw new ArgumentException("Title cannot be null or empty.", nameof(title));
			if (title.Length > 100)
				throw new ArgumentException("Title cannot exceed 100 characters.", nameof(title));
			Title = title;
        }

        public void AddQuestion(Question question)
        {
            if (Questions.Count >= 5)
                throw new InvalidOperationException("Quiz cannot have more than 5 questions.");

            Questions.Add(question);
        }
    }
}

