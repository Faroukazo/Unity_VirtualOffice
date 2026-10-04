using System.IO;
using System.IO.Compression;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class DesktopCaptureManager : NetworkBehaviour
{
    Texture2D tex;
    public RawImage outputImage;
    private float captureInterval = 1.0f;
    private float nextCaptureTime = 0f;
    private int captureWidth = 64;
    private int captureHeight = 36;
    
    public bool isScreenSharing = false;

    void Start()
    {
        if (isLocalPlayer)
        {
            tex = new Texture2D(captureWidth, captureHeight);
        }
    }

    void Update()
    {
        if (isLocalPlayer && isScreenSharing && Time.time >= nextCaptureTime)
        {
            nextCaptureTime = Time.time + captureInterval;  
            CaptureScreen();  
            byte[] imageBytes = tex.EncodeToJPG(10);  
            byte[] compressed = Compress(imageBytes);
            CmdSendScreenCapture(compressed);
        }
    }

    void CaptureScreen()
    {
        tex = ScreenCapture.CaptureScreenshotAsTexture();
        Texture2D resizedTexture = new Texture2D(captureWidth, captureHeight);
        Graphics.ConvertTexture(tex, resizedTexture);  
        tex = resizedTexture;
        outputImage.texture = tex;
    }

    [Command]
    void CmdSendScreenCapture(byte[] imageBytes)
    {
        RpcReceiveScreenCapture(imageBytes);  
    }

    [ClientRpc]
    void RpcReceiveScreenCapture(byte[] imageBytes)
    {
        byte[] decompressed = Decompress(imageBytes);   
        Texture2D receivedTexture = new Texture2D(2, 2);
        receivedTexture.LoadImage(decompressed);       
        outputImage.texture = receivedTexture;         
    }

    byte[] Compress(byte[] data)
    {
        using (var output = new MemoryStream())
        {
            using (var gzip = new GZipStream(output, CompressionMode.Compress))
            {
                gzip.Write(data, 0, data.Length);
            }
            return output.ToArray();
        }
    }

    byte[] Decompress(byte[] data)
    {
        using (var input = new MemoryStream(data))
        using (var gzip = new GZipStream(input, CompressionMode.Decompress))
        using (var output = new MemoryStream())
        {
            gzip.CopyTo(output);
            return output.ToArray();
        }
    }

    public void StartScreenSharing()
    {
        if (!isLocalPlayer || isScreenSharing)
        {
            return;
        }

        CmdUpdateScreenSharingState(true);  
    }

    public void StopScreenSharing()
    {
        if (!isLocalPlayer || !isScreenSharing)
        {
            return;
        }

        CmdUpdateScreenSharingState(false);  
    }

    public void ViewSharedScreen()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        CmdRequestSharedScreen();  
    }

    [Command]
    void CmdUpdateScreenSharingState(bool sharing)
    {
        isScreenSharing = sharing;
    }

    [Command]
    void CmdRequestSharedScreen()
    {
        if (!isScreenSharing) return;

        CaptureScreen();
        byte[] imageBytes = tex.EncodeToJPG(10);  
        byte[] compressed = Compress(imageBytes); 
        RpcReceiveScreenCapture(compressed);  
    }
}
