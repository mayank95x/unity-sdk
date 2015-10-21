﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Evidence : QuestionComponentBase {
	private List<EvidenceItem> m_EvidenceItems = new List<EvidenceItem>();
	private ObservedList<string> m_EvidenceData = new ObservedList<string>();

	new void Start()
	{
		base.Start();
	}

	new public void Init()
	{
		m_EvidenceData.Added += onAdd;
	}

	private void onAdd()
	{
		Debug.Log ("evidence added");
		EvidenceItem evidenceItem = new EvidenceItem ();
		m_EvidenceItems.Add (evidenceItem);
		evidenceItem.m_Evidence = m_EvidenceData [m_EvidenceData.Count];
	}
}
