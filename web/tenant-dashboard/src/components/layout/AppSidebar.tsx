"use client";

import { useCallback, useEffect, useRef, useState } from "react";
import Link from "next/link";
import { usePathname } from "next/navigation";
import {
  LayoutDashboard, Package, MessageSquare, Settings,
  MoreHorizontal, Users, MapPin, CalendarCheck, DollarSign,
} from "lucide-react";
import { useSidebar } from "@/context/SidebarContext";

type NavItem = {
  name: string;
  icon: React.ReactNode;
  path?: string;
  subItems?: { name: string; path: string }[];
};

const navItems: NavItem[] = [
  { icon: <LayoutDashboard size={22} />, name: "Dashboard",  path: "/" },
  { icon: <Package size={22} />,         name: "Packages",   path: "/packages" },
  { icon: <MessageSquare size={22} />,   name: "Inquiries",  path: "/inquiries" },
  { icon: <MapPin size={22} />,          name: "Tours",      path: "/tours" },
  { icon: <CalendarCheck size={22} />,   name: "Bookings",   path: "/bookings" },
  { icon: <Users size={22} />,           name: "Staff",      path: "/staff" },
  { icon: <DollarSign size={22} />,      name: "Commission", path: "/commission" },
  { icon: <Settings size={22} />,        name: "Settings",   path: "/settings" },
];

const AppSidebar: React.FC = () => {
  const { isExpanded, isMobileOpen, isHovered, setIsHovered } = useSidebar();
  const pathname = usePathname();

  const [openSubmenu, setOpenSubmenu] = useState<{ index: number } | null>(null);
  const [subMenuHeight, setSubMenuHeight] = useState<Record<string, number>>({});
  const subMenuRefs = useRef<Record<string, HTMLDivElement | null>>({});

  const isActive = useCallback(
    (path: string) =>
      path === "/" ? pathname === "/" : pathname.startsWith(path),
    [pathname]
  );

  useEffect(() => {
    let matched = false;
    navItems.forEach((nav, index) => {
      if (nav.subItems?.some((s) => isActive(s.path))) {
        setOpenSubmenu({ index });
        matched = true;
      }
    });
    if (!matched) setOpenSubmenu(null);
  }, [pathname, isActive]);

  useEffect(() => {
    if (openSubmenu !== null) {
      const key = `main-${openSubmenu.index}`;
      if (subMenuRefs.current[key]) {
        setSubMenuHeight((prev) => ({
          ...prev,
          [key]: subMenuRefs.current[key]?.scrollHeight || 0,
        }));
      }
    }
  }, [openSubmenu]);

  const showLabel = isExpanded || isHovered || isMobileOpen;

  return (
    <aside
      className={`fixed mt-16 flex flex-col lg:mt-0 top-0 px-5 left-0 bg-gray-dark border-r border-gray-800 h-screen transition-all duration-300 ease-in-out z-50
        ${isExpanded || isMobileOpen ? "w-[290px]" : isHovered ? "w-[290px]" : "w-[90px]"}
        ${isMobileOpen ? "translate-x-0" : "-translate-x-full"}
        lg:translate-x-0`}
      onMouseEnter={() => !isExpanded && setIsHovered(true)}
      onMouseLeave={() => setIsHovered(false)}
    >
      {/* Logo */}
      <div className={`py-8 flex ${!isExpanded && !isHovered ? "lg:justify-center" : "justify-start"}`}>
        <Link href="/">
          {showLabel ? (
            <div>
              <span className="text-xl font-bold text-brand-500">LitXusTravel</span>
              <p className="text-xs text-gray-500 dark:text-gray-400">Tenant Portal</p>
            </div>
          ) : (
            <span className="text-xl font-bold text-brand-500">TP</span>
          )}
        </Link>
      </div>

      {/* Nav */}
      <div className="flex flex-col overflow-y-auto duration-300 ease-linear no-scrollbar">
        <nav className="mb-6">
          <div className="flex flex-col gap-4">
            <div>
              <h2 className={`mb-4 text-xs uppercase flex leading-[20px] text-gray-500 ${!isExpanded && !isHovered ? "lg:justify-center" : "justify-start"}`}>
                {showLabel ? "Menu" : <MoreHorizontal className="size-6" />}
              </h2>
              <ul className="flex flex-col gap-4">
                {navItems.map((nav, index) => (
                  <li key={nav.name}>
                    {nav.path && (
                      <Link
                        href={nav.path}
                        className={`menu-item group ${isActive(nav.path) ? "menu-item-active" : "menu-item-inactive"}`}
                      >
                        <span className={`menu-item-icon-size ${isActive(nav.path) ? "menu-item-icon-active" : "menu-item-icon-inactive"}`}>
                          {nav.icon}
                        </span>
                        {showLabel && <span className="menu-item-text">{nav.name}</span>}
                      </Link>
                    )}
                  </li>
                ))}
              </ul>
            </div>
          </div>
        </nav>
      </div>
    </aside>
  );
};

export default AppSidebar;
