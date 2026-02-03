import { Text, View, StyleSheet } from "react-native";
import theme from "@/constants/theme";

export default function AboutScreen() {
  return (
    <View style={styles.container}>
      <Text style={styles.text}>About screen</Text>
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: theme.Colors.background,
    justifyContent: "center",
    alignItems: "center",
  },
  text: {
    color: theme.Colors.textPrimary,
  },
});
