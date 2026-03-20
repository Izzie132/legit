import { Pressable, Text, View } from "react-native";
import FontAwesome from "@expo/vector-icons/FontAwesome";

type Props = {
  label: string;
  theme?: "primary";
  onPress?: () => void;
};

export default function Button({ label, theme, onPress }: Props) {
  if (theme === "primary") {
    return (
      <View className="mx-5 h-[68px] w-[320px] items-center justify-center rounded-[18px] border-4 border-brand-primary p-[3px]">
        <Pressable
          className="w-full h-full flex-row items-center justify-center rounded-[10px] bg-white"
          onPress={onPress}
        >
          <FontAwesome
            name="picture-o"
            size={18}
            color="#78BE20"
            className="pr-2"
          />
          <Text className="text-base text-brand-background">{label}</Text>
        </Pressable>
      </View>
    );
  }

  return (
    <View className="mx-5 h-[68px] w-[320px] items-center justify-center p-[3px]">
      <Pressable
        className="w-full h-full flex-row items-center justify-center rounded-[10px]"
        onPress={onPress}
      >
        <Text className="text-base text-white">{label}</Text>
      </Pressable>
    </View>
  );
}
