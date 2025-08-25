using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using TMPro;
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
        List<Character> sortedCharacters = characters.Where(c => c.isAlive).OrderByDescending(c => c.actionGauge).ToList();

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
            turnOrderIcons.Add(icon);
        }
    }

    /// <summary>
    /// Đánh dấu nhân vật đang hoạt động.
    /// </summary>
    /// <param name="activeCharacter">Nhân vật đang có lượt đi.</param>
    public void HighlightActiveCharacter(Character activeCharacter)
    {
        // Đặt lại kích thước cho tất cả các icon về trạng thái ban đầu trước
        foreach (var icon in turnOrderIcons)
        {
            icon.transform.localScale = Vector3.one;
        }

        // Vòng lặp để làm nổi bật icon của nhân vật đang hoạt động
        for (int i = 0; i < turnOrderIcons.Count; i++)
        {
            // So sánh tên của nhân vật trong UI với tên của nhân vật đang hoạt động
            // Hiện tại, chúng ta vẫn dùng tên để đơn giản hóa
            if (turnOrderIcons[i].GetComponentInChildren<TextMeshProUGUI>().text == activeCharacter.gameObject.name)
            {
                // Thay đổi kích thước hoặc màu sắc để làm nổi bật
                turnOrderIcons[i].transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
                break; // Thêm break để chỉ làm nổi bật icon đầu tiên tìm thấy
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

        // Cập nhật văn bản cho mỗi nhân vật
        for (int i = 0; i < sortedCharacters.Count; i++)
        {
            TextMeshProUGUI[] texts = turnOrderIcons[i].GetComponentsInChildren<TextMeshProUGUI>();

            if (texts.Length > 0)
            {
                // Component đầu tiên để hiển thị tên
                texts[0].text = sortedCharacters[i].gameObject.name;

                // Nếu có component thứ hai, dùng để hiển thị điểm hành động
                if (texts.Length > 1)
                {
                    texts[1].text = Mathf.RoundToInt(sortedCharacters[i].actionGauge).ToString();
                }
            }
        }
    }
}
