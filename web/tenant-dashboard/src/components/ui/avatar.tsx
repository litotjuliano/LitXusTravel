interface AvatarProps {
  src?: string;
  alt?: string;
  size?: "xsmall" | "small" | "medium" | "large" | "xlarge";
  fallback?: string;
  className?: string;
}

const sizeClasses = {
  xsmall: "h-6 w-6 text-xs",
  small: "h-8 w-8 text-xs",
  medium: "h-10 w-10 text-sm",
  large: "h-12 w-12 text-base",
  xlarge: "h-14 w-14 text-lg",
};

const Avatar: React.FC<AvatarProps> = ({
  src,
  alt = "Avatar",
  size = "medium",
  fallback,
  className = "",
}) => {
  if (src) {
    return (
      <div className={`relative rounded-full overflow-hidden ${sizeClasses[size]} ${className}`}>
        <img src={src} alt={alt} className="object-cover w-full h-full" />
      </div>
    );
  }
  return (
    <div className={`flex items-center justify-center rounded-full bg-brand-500 text-white font-semibold ${sizeClasses[size]} ${className}`}>
      {fallback || alt?.[0]?.toUpperCase() || "?"}
    </div>
  );
};

function AvatarFallback({ children, className = "" }: { children?: React.ReactNode; className?: string }) {
  return (
    <div className={`flex items-center justify-center w-full h-full rounded-full bg-brand-500 text-white font-semibold text-sm ${className}`}>
      {children}
    </div>
  );
}

export { Avatar, AvatarFallback };
export default Avatar;
