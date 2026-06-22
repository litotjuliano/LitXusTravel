function Separator({
  orientation = "horizontal",
  className = "",
}: {
  orientation?: "horizontal" | "vertical";
  className?: string;
}) {
  return (
    <div
      className={
        orientation === "horizontal"
          ? `h-px w-full bg-gray-200 dark:bg-gray-800 ${className}`
          : `w-px self-stretch bg-gray-200 dark:bg-gray-800 ${className}`
      }
    />
  );
}

export { Separator };
