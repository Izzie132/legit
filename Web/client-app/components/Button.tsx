import { Pressable, StyleSheet, Text, View } from "react-native";
import FontAwesome from "@expo/vector-icons/FontAwesome";
import Theme from "@/constants/theme";

type Props = {
  label: string;
  theme?: "primary";
  onPress?: () => void;
};

export default function Button({ label, theme, onPress }: Props) {
  if (theme === "primary") {
    return (
      <View
        style={[
          styles.buttonContainer,
          {
            borderWidth: 4,
            borderColor: Theme.Colors.primary,
            borderRadius: 18,
          },
        ]}
      >
        <Pressable
          style={[styles.button, { backgroundColor: Theme.Colors.white }]}
          onPress={onPress}
        >
          <FontAwesome
            name="picture-o"
            size={18}
            color={Theme.Colors.primary}
            style={styles.buttonIcon}
          />
          <Text
            style={[styles.buttonLabel, { color: Theme.Colors.background }]}
          >
            {label}
          </Text>
        </Pressable>
      </View>
    );
  }

  return (
    <View style={styles.buttonContainer}>
      <Pressable style={styles.button} onPress={onPress}>
        <Text style={styles.buttonLabel}>{label}</Text>
      </Pressable>
    </View>
  );
}

const styles = StyleSheet.create({
  buttonContainer: {
    width: 320,
    height: 68,
    marginHorizontal: 20,
    alignItems: "center",
    justifyContent: "center",
    padding: 3,
  },
  button: {
    borderRadius: 10,
    width: "100%",
    height: "100%",
    alignItems: "center",
    justifyContent: "center",
    flexDirection: "row",
  },
  buttonIcon: {
    paddingRight: 8,
  },
  buttonLabel: {
    color: Theme.Colors.white,
    fontSize: 16,
  },
});
