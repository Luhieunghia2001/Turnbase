using UnityEngine;
using System.Collections.Generic;
using System.Linq; // Thêm thư viện Linq
using TMPro; // Thêm thư viện TextMeshPro
using UnityEngine.UI;

// Script này quản lý giao diện hiển thị thứ tự lượt đi
public class TurnOrderUI : MonoBehaviour
{
    // Tham chiếu đến các UI prefab để hiển thị nhân vật
    public GameObject turnOrderIconPrefab;

    // Tham chiếu đến container chứa các icon
    public Transform turnOrderContainer;

    // Danh sách các icon được tạo ra
    private List<GameObject> turnOrderIcons = new List<GameObject>();

    /// <summary>
    /// Cập nhật danh sách các nhân vật trong hàng đợi lượt đi.
    /// </summary>
    /// <param name="characters">Danh sách các nhân vật trong trận chiến.</param>
    public void UpdateTurnQueue(List<Character> characters)
    {
        // Sắp xếp các nhân vật theo thanh hành động giảm dần
        // Điều này đảm bảo rằng người có thanh hành động cao nhất sẽ hiển thị đầu tiên
        List<Character> sortedCharacters = characters.OrderByDescending(c => c.actionGauge).ToList();

        // Xóa tất cả các icon cũ
        foreach (var icon in turnOrderIcons)
        {
            Destroy(icon);
        }
        turnOrderIcons.Clear();

        // Tạo lại icon cho mỗi nhân vật
        foreach (Character character in sortedCharacters)
        {
            GameObject icon = Instantiate(turnOrderIconPrefab, turnOrderContainer);
            // Lấy component TextMeshProUGUI trên icon và gán tên nhân vật
            TextMeshProUGUI iconText = icon.GetComponentInChildren<TextMeshProUGUI>();
            if (iconText != null)
            {
                iconText.text = character.gameObject.name;
            }
            turnOrderIcons.Add(icon);
        }
    }

    /// <summary>
    /// Đánh dấu nhân vật đang hoạt động.
    /// </summary>
    /// <param name="activeCharacter">Nhân vật đang có lượt đi.</param>
    public void HighlightActiveCharacter(Character activeCharacter)
    {
        // Vòng lặp để làm nổi bật icon của nhân vật đang hoạt động
        for (int i = 0; i < turnOrderIcons.Count; i++)
        {
            // So sánh tên của nhân vật trong UI với tên của nhân vật đang hoạt động
            if (turnOrderIcons[i].GetComponentInChildren<TextMeshProUGUI>().text == activeCharacter.gameObject.name)
            {
                // Thay đổi kích thước hoặc màu sắc để làm nổi bật
                turnOrderIcons[i].transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);

                // Bạn có thể thêm các hiệu ứng khác ở đây, ví dụ như thay đổi màu
                // turnOrderIcons[i].GetComponent<Image>().color = Color.yellow;
            }
            else
            {
                // Đặt lại kích thước hoặc màu sắc ban đầu
                turnOrderIcons[i].transform.localScale = Vector3.one;
                // turnOrderIcons[i].GetComponent<Image>().color = Color.white;
            }
        }
    }

    // Thêm phương thức để cập nhật trạng thái thanh hành động
    public void UpdateActionGaugeUI(List<Character> characters)
    {
        // Lọc các nhân vật đang sống và sắp xếp theo thanh hành động giảm dần
        List<Character> sortedCharacters = characters.Where(c => c.isAlive).OrderByDescending(c => c.actionGauge).ToList();

        // Cập nhật lại UI dựa trên danh sách đã sắp xếp
        UpdateTurnQueue(sortedCharacters);

        // Tùy chọn: Thêm một vòng lặp để cập nhật thanh tiến trình nếu bạn có
        // foreach (var character in characters)
        // {
        //     // Tìm icon của nhân vật và cập nhật thanh tiến trình của nó
        // }
    }
}
