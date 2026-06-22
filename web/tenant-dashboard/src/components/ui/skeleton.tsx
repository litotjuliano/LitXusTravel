function Skeleton({ className = "", ...props }: React.ComponentProps<"div">) {
  return (
    <div
      className={`animate-pulse rounded-md bg-gray-200 dark:bg-gray-700 ${className}`}
      {...props}
    />
  );
}

export { Skeleton };
