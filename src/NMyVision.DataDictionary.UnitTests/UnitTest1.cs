using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NMyVision.DataDictionaryTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            string json = @"{
    'title': 'Person',

	'type': 'object',
    'properties': {
		'firstName': {
			'type': 'string'
		},
        'lastName': {
			'type': 'string'

		},
        'age': {
			'description': 'Age in years',
            'type': 'integer',
            'minimum': 0,
			'foo': null
		}
	},
    'required': ['firstName', 'lastName']
}";

            json = @"{  
                'required': ['firstName', 'lastName'],
                'people': [
                    { name: 'John Doe' },
                    { name: 'Jane Doe' , type: 'Person' },
                ],
                'tags' : []
            }";
            var dd = DataDictionary.ParseJson(json);

            var req = dd.Get<IEnumerable<string>>("required");
            
            Assert.AreEqual(2, req.Count(), "require array");

            var people = dd.GetItems("people");

            Assert.AreEqual(2, people.Count(), "people count");

            var tags = dd.Get<object[]>("tags");
            Assert.AreEqual(0, tags.Count());
        }
    }
}
