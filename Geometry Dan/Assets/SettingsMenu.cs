using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
	[SerializeField] private Toggle touchToggle;
	[SerializeField] private Toggle volumeToggle;
	[SerializeField] private Toggle tiltToggle;

	private void Awake()
	{
		this.touchToggle.isOn = Handler.TouchEnabled;
		this.volumeToggle.isOn = Handler.VolumeEnabled;
		this.tiltToggle.isOn = Handler.TiltEnabled;
	}

	public void SetTouchToggle()
	{
		Handler.TouchEnabled = this.touchToggle.isOn;
	}

	public void SetVolumeToggle()
	{
		Handler.VolumeEnabled = this.volumeToggle.isOn;
	}

	public void SetTiltToggle()
	{
		Handler.TiltEnabled = this.tiltToggle.isOn;
	}
}
