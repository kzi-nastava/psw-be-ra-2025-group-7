using System;
using System.Diagnostics.Eventing.Reader;
using Explorer.BuildingBlocks.Core.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Core.Domain.Entities
{
	public class Quiz : Entity
    {
		public long AuthorId { get; private set; }
		public string Title { get; private set; }

        public List<Question> Questions { get; private set; } = new();

        protected Quiz() { } // potreban za EF

        public Quiz(long authorId, string title, IEnumerable<Question> questions = null)
		{
            if (authorId <= 0)
                throw new ArgumentException("AuthorId must be a positive number.", nameof(authorId));

            AuthorId = authorId;
			SetTitle(title);

            if (questions != null)
                Questions = questions.ToList();
        }

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

