﻿/**
* Copyright 2018, 2019 IBM Corp. All Rights Reserved.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*      http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*
*/

using System.Collections;
using System.Collections.Generic;
using System.IO;
using IBM.Cloud.SDK;
using IBM.Watson.NaturalLanguageClassifier.V1;
using IBM.Watson.NaturalLanguageClassifier.V1.Model;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace IBM.Watson.Tests
{
    public class NaturalLanguageClassifierV1IntegrationTests
    {
        private NaturalLanguageClassifierService service;
        private Dictionary<string, object> customData;
        private Dictionary<string, string> customHeaders = new Dictionary<string, string>();
        private string classifierId;
        private string createdClassifierId;
        private string classifierDataFilePath;
        private string metadataDataFilePath;
        private string textToClassify0 = "Is it raining?";
        private string textToClassify1 = "Will it be hot today?";

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            LogSystem.InstallDefaultReactors();
            classifierDataFilePath = Application.dataPath + "/Watson/Tests/TestData/NaturalLanguageClassifierV1/weather-data.csv";
            metadataDataFilePath = Application.dataPath + "/Watson/Tests/TestData/NaturalLanguageClassifierV1/metadata.json";
            customHeaders.Add("X-Watson-Test", "1");
        }

        [UnitySetUp]
        public IEnumerator UnityTestSetup()
        {
            if (service == null)
            {
                service = new NaturalLanguageClassifierService();
            }

            while (!service.Credentials.HasIamTokenData())
                yield return null;
        }

        [SetUp]
        public void TestSetup()
        {
            customData = new Dictionary<string, object>();
            customData.Add(Constants.String.CUSTOM_REQUEST_HEADERS, customHeaders);
        }

        #region ListClassifiers
        [UnityTest, Order(0)]
        public IEnumerator TestListClassifiers()
        {
            Log.Debug("NaturalLanguageClassifierServiceV1IntegrationTests", "Attempting to ListClassifiers...");
            ClassifierList listClassifiersResponse = null;
            service.ListClassifiers(
                callback: (DetailedResponse<ClassifierList> response, IBMError error, Dictionary<string, object> customResponseData) =>
                {
                    Log.Debug("NaturalLanguageClassifierServiceV1IntegrationTests", "ListClassifiers result: {0}", customResponseData["json"].ToString());
                    listClassifiersResponse = response.Result;
                    classifierId = listClassifiersResponse.Classifiers[0].ClassifierId;
                    Assert.IsNotNull(listClassifiersResponse);
                    Assert.IsNotNull(listClassifiersResponse.Classifiers);
                    Assert.IsTrue(listClassifiersResponse.Classifiers.Count > 0);
                    Assert.IsNull(error);
                },
                customData: customData
            );

            while (listClassifiersResponse == null)
                yield return null;
        }
        #endregion

        #region Classify
        [UnityTest, Order(1)]
        public IEnumerator TestClassify()
        {
            Log.Debug("NaturalLanguageClassifierServiceV1IntegrationTests", "Attempting to Classify...");
            Classification classifyResponse = null;
            service.Classify(
                callback: (DetailedResponse<Classification> response, IBMError error, Dictionary<string, object> customResponseData) =>
                {
                    Log.Debug("NaturalLanguageClassifierServiceV1IntegrationTests", "Classify result: {0}", customResponseData["json"].ToString());
                    classifyResponse = response.Result;
                    Assert.IsNotNull(classifyResponse);
                    Assert.IsNotNull(classifyResponse.Classes);
                    Assert.IsTrue(classifyResponse.Classes.Count > 0);
                    Assert.IsNotNull(classifyResponse.TopClass);
                    Assert.IsTrue(classifyResponse.Text == textToClassify0);
                    Assert.IsTrue(classifyResponse.ClassifierId == classifierId);
                    Assert.IsNull(error);
                },
                classifierId: classifierId,
                text: textToClassify0,

                customData: customData
            );

            while (classifyResponse == null)
                yield return null;
        }
        #endregion

        #region ClassifyCollection
        [UnityTest, Order(2)]
        public IEnumerator TestClassifyCollection()
        {
            Log.Debug("NaturalLanguageClassifierServiceV1IntegrationTests", "Attempting to ClassifyCollection...");
            ClassificationCollection classifyCollectionResponse = null;
            List<ClassifyInput> collection = new List<ClassifyInput>()
            {
                new ClassifyInput()
                {
                    Text = textToClassify0
                },
                new ClassifyInput()
                {
                    Text = textToClassify1
                }
            };

            service.ClassifyCollection(
                callback: (DetailedResponse<ClassificationCollection> response, IBMError error, Dictionary<string, object> customResponseData) =>
                {
                    Log.Debug("NaturalLanguageClassifierServiceV1IntegrationTests", "ClassifyCollection result: {0}", customResponseData["json"].ToString());
                    classifyCollectionResponse = response.Result;
                    Assert.IsNotNull(classifyCollectionResponse);
                    Assert.IsNotNull(classifyCollectionResponse.Collection);
                    Assert.IsTrue(classifyCollectionResponse.Collection.Count > 0);
                    Assert.IsTrue(classifyCollectionResponse.ClassifierId == classifierId);
                    Assert.IsNull(error);
                },
                classifierId: classifierId,
                collection: collection,
                customData: customData
            );

            while (classifyCollectionResponse == null)
                yield return null;
        }
        #endregion

        #region CreateClassifier
        [UnityTest, Order(3)]
        public IEnumerator TestCreateClassifier()
        {
            Log.Debug("NaturalLanguageClassifierServiceV1IntegrationTests", "Attempting to CreateClassifier...");
            Classifier createClassifierResponse = null;
            using (FileStream fs0 = File.OpenRead(metadataDataFilePath))
            {
                using (FileStream fs1 = File.OpenRead(classifierDataFilePath))
                {
                    service.CreateClassifier(
                    callback: (DetailedResponse<Classifier> response, IBMError error, Dictionary<string, object> customResponseData) =>
                    {
                        Log.Debug("NaturalLanguageClassifierServiceV1IntegrationTests", "CreateClassifier result: {0}", customResponseData["json"].ToString());
                        createClassifierResponse = response.Result;
                        createdClassifierId = createClassifierResponse.ClassifierId;
                        Assert.IsNotNull(createClassifierResponse);
                        Assert.IsNotNull(createdClassifierId);
                        Assert.IsTrue(createClassifierResponse.Name == "unity-classifier-delete");
                        Assert.IsTrue(createClassifierResponse.Language == "en");
                        Assert.IsNull(error);
                    },
                    metadata: fs0,
                    trainingData: fs1,
                    customData: customData
                );

                    while (createClassifierResponse == null)
                        yield return null;
                }
            }
        }
        #endregion



        #region GetClassifier
        [UnityTest, Order(4)]
        public IEnumerator TestGetClassifier()
        {
            Log.Debug("NaturalLanguageClassifierServiceV1IntegrationTests", "Attempting to GetClassifier...");
            Classifier getClassifierResponse = null;
            service.GetClassifier(
                callback: (DetailedResponse<Classifier> response, IBMError error, Dictionary<string, object> customResponseData) =>
                {
                    Log.Debug("NaturalLanguageClassifierServiceV1IntegrationTests", "GetClassifier result: {0}", customResponseData["json"].ToString());
                    getClassifierResponse = response.Result;
                    Assert.IsNotNull(getClassifierResponse);
                    Assert.IsTrue(getClassifierResponse.Name == "unity-classifier-delete");
                    Assert.IsTrue(getClassifierResponse.Language == "en");
                    Assert.IsNull(error);
                },
                classifierId: createdClassifierId,
                customData: customData
            );

            while (getClassifierResponse == null)
                yield return null;
        }
        #endregion

        #region DeleteClassifier
        [UnityTest, Order(99)]
        public IEnumerator TestDeleteClassifier()
        {
            Log.Debug("NaturalLanguageClassifierServiceV1IntegrationTests", "Attempting to DeleteClassifier...");
            object deleteClassifierResponse = null;
            service.DeleteClassifier(
                callback: (DetailedResponse<object> response, IBMError error, Dictionary<string, object> customResponseData) =>
                {
                    Log.Debug("NaturalLanguageClassifierServiceV1IntegrationTests", "DeleteClassifier result: {0}", customResponseData["json"].ToString());
                    deleteClassifierResponse = response.Result;
                    Assert.IsNotNull(deleteClassifierResponse);
                    Assert.IsNull(error);
                },
                classifierId: createdClassifierId,
                customData: customData
            );

            while (deleteClassifierResponse == null)
                yield return null;
        }
        #endregion
    }
}
