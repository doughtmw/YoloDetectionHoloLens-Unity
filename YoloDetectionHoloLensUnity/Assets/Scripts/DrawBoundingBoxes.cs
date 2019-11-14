using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if ENABLE_WINMD_SUPPORT
using YoloRuntime;
#endif

/// <summary>
/// DrawingUtils namespace from personal git repo.
/// https://github.com/doughtmw/BoundingBoxUtils-Unity
/// </summary>
namespace DrawingUtils
{
    public class DrawBoundingBoxes : MonoBehaviour
    {
        public Vector2 textureSize;

        public GameObject textBoxPrefab;

        private Material _material;
        private Texture2D _texture;
        private GameObject _thisBoundingBox;

        // Start is called before the first frame update
        public void InitDrawBoundingBoxes()
        {
            if (_texture != null)
                Destroy(_texture);

            // Get material component from attached game object via the mesh renderer.
            _material = this.gameObject.GetComponent<MeshRenderer>().material;

            // Create a new texture instance with same size as the canvas.
            _texture = new Texture2D((int)textureSize.x, (int)textureSize.y);

            // Set the texture to transparent (with helper method)
            _texture = Texture2DExtension.TransparentTexture(_texture);

            // Apply and set main material texture;
            _texture.Apply();
            _material.mainTexture = _texture;
        }

#if ENABLE_WINMD_SUPPORT
        public void DrawBoxes(List<BoundingBox> boxes)
        {
            // Destroy cached variables to prevent memory leaks.
            if (_texture != null)
                Destroy(_texture);

            if (_thisBoundingBox != null)
                Destroy(_thisBoundingBox);

            // Create a new texture instance with same size as the canvas.
            _texture = new Texture2D((int)textureSize.x, (int)textureSize.y);

            // Set the texture to transparent (with helper method)
            _texture = Texture2DExtension.TransparentTexture(_texture);

            // Draw bounding boxes at specified coordinates.
            foreach (var box in boxes)
            {
                // Only draw boxes over a certain size
                if (box.Height < 50 || box.Width < 50)
                    continue;

                // Check boundary conditions
                // condition ? result_if_true : result_if_false
                // Add 2 extra pixels to boundary to prevent texture wrap
                int x1 = box.X > 0.0f ? (int)box.X : 0 + 2;
                int y1 = box.Y > 0.0f ? (int)box.Y : 0 + 2;
                int x2 = (box.Width + x1) > textureSize.x ? (int)(textureSize.x) - 2 : (int)(box.Width + x1);
                int y2 = (box.Height + y1) > textureSize.y ? (int)(textureSize.y) - 2 : (int)(box.Height + y1);

                // fit to current canvas and webcam size
                // 416 x 416 is size of tensor input
                x1 = (int)(textureSize.x * x1 / 416f);
                y1 = (int)(textureSize.y * y1 / 416f);
                x2 = (int)(textureSize.x * x2 / 416f);
                y2 = (int)(textureSize.y * y2 / 416f);

                var topLeft = new Vector2(x1, y1);
                var bottomRight = new Vector2(x2, y2);

                Debug.LogFormat("topLeft: {0}, {1}; bottomRight: {2}, {3}",
                    topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);

                _texture = Texture2DExtension.Box(
                    _texture,
                    topLeft,
                    bottomRight,
                    Color.cyan);

                // Create a new 3D text object at position and
                // set the label string.Canvas is scaled to x = -0.5, 5
                // and y = -0.5, 0.5.
                var xText = ((topLeft.x / textureSize.x) - 0.5f) + 0.01f;
                var yText = 0.5f - (1.0f - (topLeft.y / textureSize.y));
                _thisBoundingBox = Instantiate(
                    textBoxPrefab,
                    Vector3.zero,
                    Quaternion.identity,
                    this.gameObject.transform) as GameObject;

                _thisBoundingBox.transform.localPosition = new Vector3(xText, yText, 0f);

                // Set the label of the bounding box.
                var label = $"{box.Label}: {box.Confidence} ";
                _thisBoundingBox.GetComponent<TextMesh>().text = label;
                _thisBoundingBox.GetComponent<TextMesh>().color = Color.cyan;
            }

            // Apply and set main material texture;
            _texture.Apply();
            _material.mainTexture = _texture;
        }
#endif

    }
}
