using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoEnd : MonoBehaviour
{
    public VideoPlayer vp;

    private void Awake()
    {
        vp = GetComponent<VideoPlayer>();
    }

    public void isVideoEnd()
    {
        vp.playbackSpeed = vp.playbackSpeed / 10.0F;
    }}
