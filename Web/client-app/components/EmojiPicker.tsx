import { Modal, View, Text, Pressable } from "react-native";
import { PropsWithChildren } from "react";
import MaterialIcons from "@expo/vector-icons/MaterialIcons";

type Props = PropsWithChildren<{
  isVisible: boolean;
  onClose: () => void;
}>;

export default function EmojiPicker({ isVisible, children, onClose }: Props) {
  return (
    <View>
      <Modal animationType="slide" transparent={true} visible={isVisible}>
        <View className="absolute bottom-0 h-1/4 w-full rounded-t-[18px] bg-brand-background">
          <View className="h-[16%] flex-row items-center justify-between rounded-t-[10px] bg-brand-primary px-5">
            <Text className="text-base text-brand-background">
              Choose a sticker
            </Text>
            <Pressable onPress={onClose}>
              <MaterialIcons name="close" color="#fff" size={22} />
            </Pressable>
          </View>
          {children}
        </View>
      </Modal>
    </View>
  );
}
