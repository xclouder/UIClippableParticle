/************************************************************
//     文件名      : UIClippableParticle.cs
//     功能描述    : 
//     负责人      : cai yang
//     参考文档    : 无
//     创建日期    : 11/28/2016
//     Copyright  : Copyright 2016-2017 EZFun.
**************************************************************/

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(ParticleSystem))]
public class UIClippableParticle2 : UIBehaviour, IClippable {

	private Renderer m_renderer;
	public Renderer ParticleRenderer
	{
		get {
			if (m_renderer == null)
				m_renderer = GetComponent<Renderer>();

			return m_renderer;
		}
	}

	private RectTransform m_rectTr;

	private RectMask2D m_ParentMask;

	protected override void Start ()
	{
		base.Start ();

		m_rectTr = GetComponent<RectTransform>();
	}

	public RectTransform rectTransform {
		get {
			if (m_rectTr == null)
				m_rectTr = GetComponent<RectTransform>();

			return m_rectTr;
		}
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		UpdateClipParent();
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		UpdateClipParent();
	}

	public void Cull (Rect clipRect, bool validRect)
	{
		
		if (validRect)
			ParticleRenderer.material.SetFloat("_EnableClip", 1);
		else
			ParticleRenderer.material.SetFloat("_EnableClip", 0);

		if (!validRect)
			return;

		//Debug.LogWarning("clippable rect:" + clipRect);
		var mtx = canvas.transform.localToWorldMatrix;

		var worldMin = mtx.MultiplyPoint(clipRect.min);
		var worldMax = mtx.MultiplyPoint(clipRect.max);

		Vector4 vec = new Vector4(worldMin.x, worldMin.y, worldMax.x, worldMax.y);

		//Debug.LogWarning("clippable vec:" + vec);
		ParticleRenderer.material.SetVector("_ClipRect", vec);
	}

	private Canvas m_canvas;
	public Canvas canvas
	{
		get
		{
			//获得父节点的canvas，主要为了从canvasSpace rect转换到worldSpace
			//参考RectMask2D.canvasRect，里面也是从父节点找一个Canvas，从而设置Canvas.cullRect
			//虽然内部使用了ListPool来优化性能，但这样遍历查找性能仍然不会很理想，这可能是uGUI设计时的失误。
			//NOTE:RectMask2D.PerformClipping里面执行Clipping.FindCullAndClipWorldRect，频率是很高的
			if (m_canvas == null)
				m_canvas = gameObject.GetComponentInParent<Canvas>();

			return m_canvas;
		}
	}

	public void RecalculateClipping ()
	{
		UpdateClipParent();
	}

	private void UpdateClipParent()
	{
		var newParent = (IsActive()) ? MaskUtilities.GetRectMaskForClippable(this) : null;
	
		if (newParent != m_ParentMask && m_ParentMask != null)
			m_ParentMask.RemoveClippable(this);
		
		if (newParent != null)
			newParent.AddClippable(this);

		m_ParentMask = newParent;
	}

	public void SetClipRect (Rect value, bool validRect)
	{
		//do nothing
		if (validRect)
			ParticleRenderer.sharedMaterial.SetFloat("_EnableClip", 1);
		else
			ParticleRenderer.sharedMaterial.SetFloat("_EnableClip", 0);
	}

}
