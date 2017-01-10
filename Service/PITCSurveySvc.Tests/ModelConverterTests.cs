using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PITCSurveySvc.Models;
using PITCSurveyEntities.Entities;
using System.Web.Script.Serialization;
using PITCSurveyLib.Models;
using System.IO;
using System.Linq;

namespace PITCSurveySvc.Tests
{
	[TestClass]
	public class ModelConverterTests
	{
		[TestMethod]
		public void TestImportSurveyJSON()
		{
			using (PITCSurveyContext db = new PITCSurveyContext())
			{
				ModelConverter mc = new ModelConverter(db);

				var js = new JavaScriptSerializer();

				SurveyModel Model = js.Deserialize<SurveyModel>(File.ReadAllText(".\\Data\\PITYouthCountSurvey.json"));

				Survey Survey = mc.ConvertToEntity(Model);

				Console.WriteLine();
			}
		}

		[TestMethod]
		public void TestBuildResponseJSON()
		{
			using (var db = new PITCSurveyEntities.Entities.PITCSurveyContext())
			{
				var Survey = db.Surveys.Where(s => s.ID == 1).Single();

				var Responses = TestHelpers.GenerateRandomResponseForSurvey(Survey);

				var js = new JavaScriptSerializer();

				System.Diagnostics.Trace.WriteLine(js.Serialize(Responses));
			}
		}
	}
}
