def analyze_priority(color, description):
    colorMarker = {"red": 1, "yellow": 2, "blue": 3, "green": 4}
    keywords = ["urgent", "important", "priority", "critical"]

    priority = 0

    priority = colorMarker.get(color.lower())

    for word in keywords:
        if word in description.lower():
            priority = 1

    return priority