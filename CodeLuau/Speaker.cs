﻿using CodeLuau;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeLuau
{
 
	public class Speaker
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }
		public int? YearsExpirience { get; set; }
		public bool HasBlog { get; set; }
		public string BlogURL { get; set; }
		public WebBrowser Browser { get; set; }
		public List<string> Certifications { get; set; }
		public string Employer { get; set; }
		public int RegistrationFee { get; set; }
		public List<Session> Sessions { get; set; }
 
		public RegisterResponse Register(IRepository repository)
		{

			
			var error = ValidateRegistration();
			if (error != null) return new RegisterResponse(error);
            int? speakerId = repository.SaveSpeaker(this);
			return new RegisterResponse((int)speakerId);

		}
		  
		private RegisterError? ValidateRegistration()
		{
            var error = ValidateData();
            if (error != null) return error;

            //put list of employers in array
            bool speakerAppearsQulified = AppearsExeptional() || !HasObiousRedFlags();

            if (!speakerAppearsQulified)
            {
                return  RegisterError.SpeakerDoesNotMeetStandards;
            }
            bool atLeastOneSessionApproved = approveSessions();
			if(!atLeastOneSessionApproved) return RegisterError.NoSessionsApproved;
			return null;
        }

        private bool approveSessions()
		{
			foreach (var session in Sessions)
			{

				session.Approved = !SessionIsAboutOldTechnology(session);
				
			}

			return Sessions.Any(s => s.Approved);
		}

		private bool SessionIsAboutOldTechnology(Session session)
		{
            var oldTechnologies = new List<string>() { "Cobol", "Punch Cards", "Commodore", "VBScript" };
           
			foreach (var tech in oldTechnologies)
            {
                if (session.Title.Contains(tech) || session.Description.Contains(tech))
                {
                   return true;
                }
              
            }
			return false;
        }

        private bool HasObiousRedFlags()
		{ 
			string emailDomain = Email.Split('@').Last();
			var acientEmailDomain = new List<string>() { "aol.com", "prodigy.com", "compuserve.com" };

			if (acientEmailDomain.Contains(emailDomain)) return true;
			if (Browser.Name == WebBrowser.BrowserName.InternetExplorer && Browser.MajorVersion < 9) return true;

			return false;

		}

		private bool AppearsExeptional()
		{

			if (YearsExpirience > 10) return true;
            if (HasBlog) return true;
            if (Certifications.Count() > 3) return true;
            var preferredEmployers = new List<string>() { "Pluralsight", "Microsoft", "Google" };
			if(preferredEmployers.Contains(Employer)) return true;

		 
			return false;
		}

		private RegisterError? ValidateData()
			{

				if (string.IsNullOrWhiteSpace(FirstName)) return RegisterError.FirstNameRequired;
				if (string.IsNullOrWhiteSpace(LastName)) return RegisterError.LastNameRequired;
				if (string.IsNullOrWhiteSpace(Email)) return RegisterError.EmailRequired;
                if (!Sessions.Any()) return  RegisterError.NoSessionsProvided;


               return null;


			}
	}
}