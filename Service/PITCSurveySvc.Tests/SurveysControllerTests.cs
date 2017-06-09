using Microsoft.VisualStudio.TestTools.UnitTesting;
using PITCSurveyEntities.Entities;

namespace PITCSurveySvc.Tests
{
    [TestClass]
    public class SurveysControllerTests
    {
        private PITCSurveyContext _db;

        [TestInitialize]
        public void Init()
        {
            EffortProviderFactory.ResetDB();
        }

        public SurveysControllerTests()
        {
            //DbConnection conn = DbConnectionFactory.CreateTransient();
            //_db = new PITCSurveyContext(conn);
            _db = new PITCSurveyContext();
        }

        [TestMethod]
        public void GetSurveyByID()
        {
            //
        }
    }
}
