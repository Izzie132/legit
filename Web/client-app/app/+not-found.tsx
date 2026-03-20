import { View } from "react-native";
import { Link, Stack } from "expo-router";

export default function NotFoundScreen() {
  return (
    <>
      <Stack.Screen options={{ title: "Oops! Not Found" }} />
      <View className="flex-1 items-center justify-center bg-brand-background">
        <Link href="/" className="text-xl text-white underline">
          Go back to Home screen!
        </Link>
      </View>
    </>
  );
}
