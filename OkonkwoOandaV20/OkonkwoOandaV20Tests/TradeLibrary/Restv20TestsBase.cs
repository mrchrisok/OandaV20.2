﻿namespace OkonkwoOandaV20Tests.TradeLibrary
{
   public class Restv20TestsBase
   {
	  static protected readonly Restv20TestResults m_Results;
	  protected short m_FailedTests = 0;

	  public Restv20TestResults Results { get { return m_Results; } }
	  public short Failures { get { return m_FailedTests; } }

	  static Restv20TestsBase()
	  {
		 m_Results = new Restv20TestResults();
	  }
   }
}
